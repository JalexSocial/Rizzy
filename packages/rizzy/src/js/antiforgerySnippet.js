const INIT_KEY = "__rizzy_htmx_antiforgery_initialized";

if (!document[INIT_KEY]) {
    document[INIT_KEY] = true;
    
    document.addEventListener("htmx:config:request", evt => {
        const request = evt.detail?.ctx?.request;
        if (!request || request.method?.toUpperCase() === 'GET') return;

        const antiforgery = htmx.config?.antiforgery;
        if (!antiforgery) return;

        const { headerName, requestToken, formFieldName } = antiforgery;

        // Ensure at least one configuration field is provided
        if (!headerName && !formFieldName) {
            console.warn("Antiforgery configuration is missing both headerName and formFieldName. Token not added.");
            return;
        }

        // If formFieldName exists and token is already set, avoid overriding it
        if (formFieldName && request.body && typeof request.body.has === 'function' && request.body.has(formFieldName)) return;

        // Apply the token either to headers or parameters based on configuration
        if (headerName) {
            request.headers[headerName] = requestToken;
        } else if (formFieldName && request.body && typeof request.body.append === 'function') {
            request.body.append(formFieldName, requestToken);
        }
    });
    document.addEventListener("htmx:after:request", evt => {
        if (evt.detail.boosted) {
            const responseText = evt.detail.ctx.text;
            const selector = 'meta[name=htmx-config]';

            // Find the index of the opening and closing tag for the meta element.
            const startIndex = responseText.indexOf(`<meta name="htmx-config"`);
            const endIndex = responseText.indexOf(">", startIndex) + 1; // Include the closing >
            const closingTagIndex = responseText.indexOf("</meta>", endIndex);  //handle self closing tags

            if (startIndex > -1) {
                let metaTagString = "";
                if(closingTagIndex > -1){
                    const closingSlashIndex = responseText.indexOf("/", startIndex);
                    if (closingSlashIndex > -1 && closingSlashIndex < endIndex) {
                        // Handle Self closing tag e.g. <meta ... />
                        metaTagString = responseText.substring(startIndex, endIndex);
                    } else {
                        metaTagString = responseText.substring(startIndex, responseText.indexOf("</meta>") + 7);
                    }
                } else {
                    metaTagString = responseText.substring(startIndex, endIndex);
                }

                // Extract the content attribute.
                const contentAttributeRegex = /content="([^"]*)"/;
                const contentMatch = metaTagString.match(contentAttributeRegex);

                if (contentMatch && contentMatch[1]) {
                    const contentValue = contentMatch[1];

                    const current = document.querySelector(selector);

                    // only change the anti-forgery token
                    const key = 'antiforgery';
                    try {
                        htmx.config[key] = JSON.parse(contentValue)[key];
                    } catch (e) {
                        console.error("Error parsing htmx-config JSON:", e);
                        return; // Exit if parsing fails
                    }


                    // Create the new meta element and set its attributes.
                    const newMeta = document.createElement('meta');
                    newMeta.setAttribute('name', 'htmx-config');
                    newMeta.setAttribute('content', contentValue);

                    // update DOM
                    current.replaceWith(newMeta);

                }
            }
        }
    });
    
}
