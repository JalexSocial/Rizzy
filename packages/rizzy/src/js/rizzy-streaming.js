/*
 * Blazor Stream Rendering HTMX Extension
 * Author: Michael Tanczos
 * Credits to SSE extension and Microsoft aspnetcore
 * at https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web.JS/src/Rendering/StreamingRendering.ts
 */

(function () {
    let api;
    let enableDomPreservation = true;

    class BlazorStreamingUpdate extends HTMLElement {
        connectedCallback() {
            const blazorSsrElement = this.parentNode;
            blazorSsrElement.parentNode?.removeChild(blazorSsrElement);

            blazorSsrElement.childNodes.forEach(node => {
                if (node instanceof HTMLTemplateElement) {
                    const componentId = node.getAttribute("blazor-component-id");
                    if (componentId) {
                        insertStreamingContentIntoDocument(componentId, node.content);
                    }
                }
            });

            htmx?.process(document.body);
        }
    }

    htmx.registerExtension("rizzy-streaming", {
        init: function (apiRef) {
            api = apiRef;

            if (htmx.blazorSwapSsr == null) {
                if (customElements.get("blazor-ssr-end") === undefined) {
                    customElements.define("blazor-ssr-end", BlazorStreamingUpdate);
                }
                htmx.blazorSwapSsr = blazorSwapSsr;
            }
        },

        htmx_after_init: function () {
            htmx?.process(document.body);
            return true;
        },

        htmx_before_request: function (elt, detail) {
            const swapSpec = api.getSwapSpecification(elt);
            const originalFetch = detail.ctx.fetch || window.fetch;

            detail.ctx.fetch = async function (url, options) {
                const response = await originalFetch(url, options);
                if (!response.body) {
                    return response;
                }

                const reader = response.body.getReader();
                const decoder = new TextDecoder();

                let container = null;
                const cid = "ctr" + crypto.randomUUID();

                while (true) {
                    const { done, value } = await reader.read();
                    if (done) {
                        if (container) {
                            unwrap(container);
                        }
                        break;
                    }

                    const chunk = decoder.decode(value, { stream: true });

                    if (!container) {
                        container = document.createElement("div");
                        container.id = cid;
                        swap(elt, container.outerHTML, swapSpec);
                        container = document.getElementById(cid) ?? container;
                    }

                    swap(container, chunk, { ...swapSpec, swapStyle: "beforeend", settleDelay: 0 });
                }

                detail.ctx.swap = "none";
                detail.ctx.text = "";
                return new Response("", { status: response.status, statusText: response.statusText, headers: response.headers });
            };

            return true;
        }
    });

    function isCommentNodeInHead(commentNode) {
        if (commentNode && commentNode.nodeType === Node.COMMENT_NODE) {
            let currentNode = commentNode.parentNode;
            while (currentNode !== null) {
                if (currentNode === document.head) {
                    return true;
                }
                currentNode = currentNode.parentNode;
            }
            return false;
        }

        return false;
    }

    function blazorSwapSsr(start, end, docFrag) {
        const newDiv = wrap(start, end, "ssr" + crypto.randomUUID());
        const container = document.createElement("div");
        container.appendChild(docFrag);
        swap(newDiv, container.innerHTML);
        unwrap(newDiv);
    }

    function wrap(start, end, id) {
        const newDiv = document.createElement("div");
        newDiv.id = id;

        let currentNode = start.nextSibling;
        while (currentNode && currentNode !== end) {
            newDiv.appendChild(currentNode);
            currentNode = start.nextSibling;
        }

        start.parentNode.insertBefore(newDiv, end);

        return newDiv;
    }

    function unwrap(element) {
        if (element.parentNode) {
            while (element.firstChild) {
                element.parentNode.insertBefore(element.firstChild, element);
            }

            element.parentNode.removeChild(element);
        }
    }

    function swap(elt, content, swapSpec) {
        const target = api.getTarget(elt);
        api.swap(target, content, swapSpec ?? api.getSwapSpecification(elt));
    }

    function insertStreamingContentIntoDocument(componentIdAsString, docFrag) {
        const markers = findStreamingMarkers(componentIdAsString);
        if (markers) {
            const { startMarker, endMarker } = markers;
            enableDomPreservation = !isCommentNodeInHead(startMarker);
            if (enableDomPreservation) {
                blazorSwapSsr(startMarker, endMarker, docFrag);
            } else {
                const destinationRoot = endMarker.parentNode;
                const existingContent = new Range();
                existingContent.setStart(startMarker, startMarker.textContent.length);
                existingContent.setEnd(endMarker, 0);
                existingContent.deleteContents();

                while (docFrag.childNodes[0]) {
                    destinationRoot.insertBefore(docFrag.childNodes[0], endMarker);
                }
            }
        }
    }

    function findStreamingMarkers(componentIdAsString) {
        const expectedStartText = `bl:${componentIdAsString}`;
        const iterator = document.createNodeIterator(document, NodeFilter.SHOW_COMMENT);

        let startMarker = null;
        while ((startMarker = iterator.nextNode())) {
            if (startMarker.textContent === expectedStartText) {
                break;
            }
        }

        if (!startMarker) {
            return null;
        }

        const expectedEndText = `/bl:${componentIdAsString}`;
        let endMarker = null;
        while ((endMarker = iterator.nextNode())) {
            if (endMarker.textContent === expectedEndText) {
                break;
            }
        }

        return endMarker ? { startMarker, endMarker } : null;
    }
})();
