import htmx from 'htmx';

(function() {
    
    htmx.defineExtension('rizzy-nonce',
        {
            transformResponse: function(text, xhr, elt) {

                let documentNonce = htmx.config.documentNonce ?? htmx.config.inlineScriptNonce;

                if (!documentNonce) {
                    console.warn("rizzy-nonce extension loaded but no no nonce found for document. Inline scripts may be blocked.");
                    documentNonce = "";
                }

                // disable ajax fetching on history miss because it doesn't handle nonce replacment
                htmx.config.refreshOnHistoryMiss = true; 

                // CSP nonce determination based on safe-nonce by Michael West
                let nonce = xhr?.getResponseHeader('HX-Nonce');
                if (!nonce) {
                    const csp = xhr?.getResponseHeader('content-security-policy');
                    if (csp) {
                        const cspMatch = csp.match(/(style|script)-src[^;]*'nonce-([^']*)'/i);
                        if (cspMatch) {
                            nonce = cspMatch[2];
                        }
                    }
                }
                if (xhr && window.location.hostname) {
                    const responseURL = new URL(xhr.responseURL);
                    if (responseURL.hostname !== window.location.hostname) {
                        nonce = ''; // ignore nonce header if request is not some domain 
                    }
                }

                nonce ??= '';

                return this.processUnsafeHtml(text, documentNonce, nonce);
            },
            processUnsafeHtml: function(text, documentNonce, newScriptNonce) {
                //const noncePattern = new RegExp(`(['"])${newScriptNonce}\\1`, 'gi');

                // Replace any occurrences of the nonce provided by the server with
                // the existing document nonce. Note that at minimum the server text originates
                // from is same-origin and the newScriptNonce that is replaced is determined
                // from response headers which are only available when processing the xmlHttpRequest
                if (documentNonce)
                    text = text.replaceAll(newScriptNonce, documentNonce);

                const parser = new DOMParser();

                try {
                    // At this point any remaining elements that don't have the correct
                    // nonce will cause console errors to be emitted. We are going to strip
                    // out those elements and any attempts to block rizzy-nonce in the included markup.
                    let doc = parser.parseFromString(text, "text/html");

                    if (doc) {
                        // Remove any attempts to disable rizzy-nonce extension
                        Array.from(doc.querySelectorAll('[hx-ext*="ignore:rizzy-nonce"], [data-hx-ext*="ignore:rizzy-nonce"]'))
                            .forEach((elt) => {
                                elt.remove();
                            });

                        // Select all <script> and <style> tags
                        const elements = doc.querySelectorAll("script, style, link");

                        // Iterate through each element
                        elements.forEach(elt => {
                            const nonce = elt.getAttribute("nonce");
                            if (nonce !== documentNonce) {
                                // Remove the element if the nonce doesn't match (or is missing)
                                elt.remove();
                            }
                        });

                        // Serialize the document back into an HTML string and return it
                        return doc.documentElement.outerHTML;
                    }

                } catch (_) { }
                {

                }

                return '';

            }
        });

})()