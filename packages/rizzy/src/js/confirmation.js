const confirm = (props = {}) => {
    return new Promise((resolve, reject) => {

        var result = {
            isConfirmed: false,
            isDenied: false,
            isDismissed: false,
            value: null
        };

        const resultEvents = {
            confirm: function (value) {
                result.isConfirmed = true;
                result.value = value;
                resolve(result);
            },
            deny: function () {
                result.isDenied = true;
                resolve(result);
            },
            dismiss: function () {
                result.isDismissed = true;
                resolve(result);
            }
        }

        const defaultEventData = {
            title: 'Proceed?',
            text: '',
            showConfirmButton: true,
            showDenyButton: false,
            showCancelButton: false,
            confirmButtonText: 'OK',
            denyButtonText: 'No',
            dismissButtonText: 'Cancel'
        };

        const eventData = { ...defaultEventData, ...props, ...resultEvents };

        const customEvent = new CustomEvent('rz:confirm-dialog', {
            detail: eventData
        });

        // Dispatch the event on the window object
        window.dispatchEvent(customEvent);
    });
};

document.addEventListener('htmx:confirm', function (evt) {

    const confirmMessage = evt.detail.elt.getAttribute("hx-confirm");

    if (confirmMessage) {
        evt.preventDefault();

        let props;

        try {
            // Attempt to parse evt.detail.question as JSON
            props = JSON.parse(evt.detail.question);
        } catch (error) {
            // If parsing fails, create an object with a "text" property
            props = {
                title: "Proceed?",
                text: evt.detail.question
            };
        }
        confirm(props).then(function (result) {
            if (result.isConfirmed)
                evt.detail.issueRequest(true);
        });
    }
});

export default confirm;