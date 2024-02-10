﻿/*
 * Blazor Stream Rendering HTMX Extension
 * Author: Michael Tanczos
 * Credits to SSE extension and Microsoft aspnetcore
 * at https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web.JS/src/Rendering/StreamingRendering.ts
 */
(function () {

    var api;
    var enableDomPreservation = true;
    var componentLoaded = false;

    class BlazorStreamingUpdate extends HTMLElement {
        connectedCallback() {
            const blazorSsrElement = this.parentNode

            // Synchronously remove this from the DOM to minimize our chance of affecting anything else
            blazorSsrElement.parentNode?.removeChild(blazorSsrElement)

            // When this element receives content, if it's <template blazor-component-id="...">...</template>,
            // insert the template content into the DOM
            blazorSsrElement.childNodes.forEach(node => {
                if (node instanceof HTMLTemplateElement) {
                    const componentId = node.getAttribute("blazor-component-id")
                    if (componentId) {
                        insertStreamingContentIntoDocument(componentId, node.content)
                    }
                }
            })

            htmx?.process(document.body)
        }
    }

    htmx.defineExtension("blazor-streaming",
        {
            /**
             * Init saves the provided reference to the internal HTMX API.
             *
             * @param {import("../htmx").HtmxInternalApi} api
             * @returns void
             */
            init: function (apiRef) {
                // store a reference to the internal API.
                api = apiRef;

                // set a function in the public API for creating new EventSource objects
                if (htmx.blazorSwapSsr == undefined) {
                    if (customElements.get('blazor-ssr-end') === undefined) {
                        customElements.define('blazor-ssr-end', BlazorStreamingUpdate);
                    }
                    htmx.blazorSwapSsr = blazorSwapSsr;
                }
            },
            onEvent: function (name, evt) {
                if (name === "htmx:afterOnLoad") {
                    htmx?.process(document.body);
                }
                else if (name === "htmx:beforeRequest") {
                    var element = evt.detail.elt;
                    if (evt.detail.requestConfig.target) {
                        evt.detail.requestConfig.target.addEventListener("htmx:beforeSwap",
                            e => {
                                // Any html that was already streamed in could have been updated with
                                // blazor ssr content so the final xhr response can be thrown away
                                //e.detail.shouldSwap = false;
                            }, { once: true });
                    }

                    var last = 0;
                    var swapSpec = api.getSwapSpecification(element);
                    var xhr = evt.detail.xhr;

                    // Create a container id for a temporary div container. All streamed html will be placed 
                    // inside the container so that htmx swap methods work correctly
                    var cid = crypto.randomUUID();

                    xhr.addEventListener("readystatechange", () => {

                        // If finished we can unwrap the container all html was stored into
                        if (xhr.readyState === 4) {
                            var container = document.getElementById(cid);

                            if (container != null)
                                unwrap(container);
                        }
                    });

                    xhr.addEventListener("progress", e => {

                        var container = document.getElementById(cid);

                        // If the container doesn't exist we need to create it and swap it into the element
                        // target space. From here on we can stream responses into the container directly.
                        if (container == null) {
                            container = document.createElement('div');
                            container.id = cid;

                            // Swap in a container div to hold the streaming html
                            swap(element, container.outerHTML, swapSpec);

                            // The very first swap into the container can be a replacement swap
                            swapSpec.swapStyle = "innerHTML";

                            // Ensure there is always a container even if not added to the dom
                            container = document.getElementById(cid) ?? container;
                        }

                        // Compute any new html in this chunk
                        diff = e.currentTarget.response.substring(last);
                        swap(container, diff, swapSpec);

                        swapSpec.settleDelay = 0;
                        swapSpec.swapStyle = "beforeend";
                        last = e.loaded;
                    });

                }

                return true;
            }
        });

    function blazorSwapSsr(start, end, docFrag) {
        var newDiv = wrap(start, end, crypto.randomUUID());

        var container = document.createElement('div');
        container.appendChild(docFrag);

        swap(newDiv, container.innerHTML);

        unwrap(newDiv);
    }

    function wrap(start, end, id) {

        var newDiv = document.createElement('div');
        newDiv.id = id;

        // Iterate through nodes between start and end
        var currentNode = start.nextSibling;
        while (currentNode && currentNode !== end) {
            newDiv.appendChild(currentNode);
            currentNode = start.nextSibling;
        }

        start.parentNode.insertBefore(newDiv, start);

        return newDiv;
    }

    function unwrap(element) {
        // Ensure that the element has a parent
        if (element.parentNode) {
            // Move all child nodes out of the element
            while (element.firstChild) {
                element.parentNode.insertBefore(element.firstChild, element);
            }

            // Remove the empty element
            element.parentNode.removeChild(element);
        }
    }

    /**
     * @param {HTMLElement} elt
     * @param {string} content
     */
    function swap(elt, content, swapSpec) {

        api.withExtensions(elt, function (extension) {
            content = extension.transformResponse(content, null, elt);
        });

        swapSpec ??= api.getSwapSpecification(elt);
        var target = api.getTarget(elt);
        var settleInfo = api.makeSettleInfo(elt);

        api.selectAndSwap(swapSpec.swapStyle, target, elt, content, settleInfo);

        settleInfo.elts.forEach(function (elt) {
            if (elt.classList) {
                elt.classList.add(htmx.config.settlingClass);
            }
            api.triggerEvent(elt, 'htmx:beforeSettle');
        });

        // Handle settle tasks (with delay if requested)
        if (swapSpec.settleDelay > 0) {
            setTimeout(doSettle(settleInfo), swapSpec.settleDelay);
        } else {
            doSettle(settleInfo)();
        }
    }

    /**
     * doSettle mirrors much of the functionality in htmx that
     * settles elements after their content has been swapped.
     * TODO: this should be published by htmx, and not duplicated here
     * @param {import("../htmx").HtmxSettleInfo} settleInfo
     * @returns () => void
     */
    function doSettle(settleInfo) {

        return function () {
            settleInfo.tasks.forEach(function (task) {
                task.call();
            });

            settleInfo.elts.forEach(function (elt) {
                if (elt.classList) {
                    elt.classList.remove(htmx.config.settlingClass);
                }
                api.triggerEvent(elt, 'htmx:afterSettle');
            });
        }
    }

    function insertStreamingContentIntoDocument(componentIdAsString, docFrag) {
        const markers = findStreamingMarkers(componentIdAsString)
        if (markers) {
            const { startMarker, endMarker } = markers
            if (enableDomPreservation) {
                blazorSwapSsr(startMarker, endMarker, docFrag);
            } else {
                // In this mode we completely delete the old content before inserting the new content
                const destinationRoot = endMarker.parentNode
                const existingContent = new Range()
                existingContent.setStart(startMarker, startMarker.textContent.length)
                existingContent.setEnd(endMarker, 0)
                existingContent.deleteContents()

                while (docFrag.childNodes[0]) {
                    destinationRoot.insertBefore(docFrag.childNodes[0], endMarker)
                }
            }
        }
    }

    function findStreamingMarkers(componentIdAsString) {
        // Find start marker
        const expectedStartText = `bl:${componentIdAsString}`
        const iterator = document.createNodeIterator(
            document,
            NodeFilter.SHOW_COMMENT
        )
        let startMarker = null
        while ((startMarker = iterator.nextNode())) {
            if (startMarker.textContent === expectedStartText) {
                break
            }
        }

        if (!startMarker) {
            return null
        }

        // Find end marker
        const expectedEndText = `/bl:${componentIdAsString}`
        let endMarker = null
        while ((endMarker = iterator.nextNode())) {
            if (endMarker.textContent === expectedEndText) {
                break
            }
        }

        return endMarker ? { startMarker, endMarker } : null
    }
})();