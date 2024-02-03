if (!document.body.attributes.__htmx_antiforgery) {
    document.addEventListener("htmx:configRequest", evt => {
        let httpVerb = evt.detail.verb.toUpperCase();
        if (httpVerb === 'GET') return;
        let antiforgery = htmx.config.antiforgery;
        if (antiforgery) {
            // already specified on the form, short circuit
            if (evt.detail.parameters[antiforgery.formFieldName])
                return;

            if (antiforgery.headerName) {
                evt.detail.headers[antiforgery.headerName]
                    = antiforgery.requestToken;
            } else {
                evt.detail.parameters[antiforgery.formFieldName]
                    = antiforgery.requestToken;
            }
        }
    });
    document.addEventListener("htmx:afterOnLoad", evt => {
        if (evt.detail.boosted) {
            const parser = new DOMParser();
            const html = parser.parseFromString(evt.detail.xhr.responseText, 'text/html');
            const selector = 'meta[name=htmx-config]';
            const config = html.querySelector(selector);
            if (config) {
                const current = document.querySelector(selector);
                // only change the anti-forgery token
                const key = 'antiforgery';
                htmx.config[key] = JSON.parse(config.attributes['content'].value)[key];
                // update DOM, probably not necessary, but for sanity's sake
                current.replaceWith(config);
            }
        }
    });
    document.body.attributes.__htmx_antiforgery = true;
}