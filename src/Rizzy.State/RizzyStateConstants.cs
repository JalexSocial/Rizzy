namespace Rizzy.State;

public static class RizzyStateConstants
{
    public static class HttpContextItems
    {
        /// <summary>HttpContext.Items key for the state token to be rendered into the initial view.</summary>
        public const string StateForView = "Rizzy.State.ForView";
        
        /// <summary>HttpContext.Items key for the new state token to be added to response headers.</summary>
        public const string NewStateForHeader = "Rizzy.State.NewForHeader";

        /// <summary>HttpContext.Items key for the current view model instance during an htmx request.</summary>
        public const string CurrentViewModelInstance = "Rizzy.State.CurrentViewModel";
        
        /// <summary>HttpContext.Items key for the current view model's version.</summary>
        public const string CurrentVersion = "Rizzy.State.CurrentVersion";

        /// <summary>HttpContext.Items key for an updated view model that should be used for new token generation.</summary>
        public const string UpdatedViewModelForToken = "Rizzy.State.UpdatedViewModelForToken";
    }

    public static class HtmxRequestHeaders
    {
        /// <summary>Client sends this with the state token.</summary>
        public const string RZState = "rz-state"; // Form parameter name

        /// <summary>Client sends this with the JSON Patch.</summary>
        public const string RZPatch = "rz-patch"; // Form parameter name

        /// <summary>Client sends this to indicate a Rizzy stateful request.</summary>
        public const string RZRequest = "RZ-Request"; // HTTP Header

        /// <summary>Client sends this with the action payload for x-rz-on.</summary>
        public const string RZActionPayload = "rz-action-payload"; // Form parameter name
    }

    public static class HtmxResponseHeaders
    {
        /// <summary>Server sends this with the new state token.</summary>
        public const string RZState = "RZ-State"; // HTTP Header
    }

    public const string RzStateScriptTagId = "rz-state"; // Default ID for the script tag
}