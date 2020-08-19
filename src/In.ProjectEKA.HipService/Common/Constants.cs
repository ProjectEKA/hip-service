namespace In.ProjectEKA.HipService.Common
{
    public static class Constants
    {
        public const string CURRENT_VERSION = "v0.5";
        public const string PATH_SESSIONS = CURRENT_VERSION + "/sessions";
        public const string PATH_CARE_CONTEXTS_DISCOVER = CURRENT_VERSION + "/care-contexts/discover";
        public const string PATH_CONSENTS_HIP = CURRENT_VERSION + "/consents/hip/notify";
        public const string PATH_LINKS_LINK_INIT = CURRENT_VERSION + "/links/link/init";
        public const string PATH_LINKS_LINK_CONFIRM = CURRENT_VERSION + "/links/link/confirm";
        public const string PATH_HEALTH_INFORMATION_HIP_REQUEST = CURRENT_VERSION + "/health-information/hip/request";
        public const string PATH_HEART_BEAT = CURRENT_VERSION + "/heartbeat";
        public const string ON_AUTH_CONFIRM = CURRENT_VERSION + "/users/auth/on-confirm";
        public const string AUTH_CONFIRM = CURRENT_VERSION + "/users/auth/confirm";
        public const string PATH_ON_AUTH_INIT = "/" + CURRENT_VERSION + "/users/auth/on-init";
        public const string PATH_ON_ADD_CONTEXTS = "/" + CURRENT_VERSION + "/links/link/on-add-contexts";
        public static readonly string DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
        public static readonly string PATH_ON_DISCOVER = "/" + CURRENT_VERSION + "/care-contexts/on-discover";
        public static readonly string PATH_ON_LINK_INIT = "/" + CURRENT_VERSION + "/links/link/on-init";
        public static readonly string PATH_ON_LINK_CONFIRM = "/" + CURRENT_VERSION + "/links/link/on-confirm";
        public static readonly string PATH_CONSENT_ON_NOTIFY = "/" + CURRENT_VERSION + "/consents/hip/on-notify";

        public static readonly string PATH_HEALTH_INFORMATION_ON_REQUEST = "/" + CURRENT_VERSION +
                                                                           "/health-information/hip/on-request";

        public static readonly string PATH_HEALTH_INFORMATION_NOTIFY_GATEWAY = "/" + CURRENT_VERSION +
                                                                               "/health-information/notify";

        public static readonly string PATH_AUTH_CONFIRM = "/" + CURRENT_VERSION + "/users/auth/confirm";
    }
}