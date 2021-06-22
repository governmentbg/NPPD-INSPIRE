namespace Inspire.Portal.Utilities
{
    using System;
    using System.Configuration;

    using Inspire.Utilities.Extensions;

    public static class ConfigurationReader
    {
        public static bool ChangeDefaultJsonValueProviderFactory =>
            ConfigurationManager.AppSettings["ChangeDefaultJsonValueProviderFactory"].IsNotNullOrEmpty()
            && Convert.ToBoolean(ConfigurationManager.AppSettings["ChangeDefaultJsonValueProviderFactory"]);

        public static long CacheExpiration => long.Parse(ConfigurationManager.AppSettings["CacheExpiration"]);

        public static bool AntiForgeryConfigRequireSsl =>
            Convert.ToBoolean(ConfigurationManager.AppSettings["AntiForgeryConfigRequireSsl"]);

        public static string AttachmentsVirtualPath => ConfigurationManager.AppSettings["AttachmentsVirtualPath"];

        public static string AttachmentsTempDir => ConfigurationManager.AppSettings["AttachmentsTempDir"];

        public static string EncryptKey => ConfigurationManager.AppSettings["EncryptKey"] ?? string.Empty;

        public static Guid AutomationUserId => Guid.Parse(ConfigurationManager.AppSettings["AutomationUserId"]);

        public static long LinkExpirationPeriod => long.Parse(ConfigurationManager.AppSettings["LinkExpirationPeriod"]);

        public static int TrimLength => int.Parse(ConfigurationManager.AppSettings["TrimLength"]);

        public static int LeadNewsTrimLength => int.Parse(ConfigurationManager.AppSettings["LeadNewsTrimLength"]);

        public static int ProviderTrimLength => int.Parse(ConfigurationManager.AppSettings["ProviderTrimLength"]);

        public static string UserPasswordRegex => @"^(?=.*[a-zA-Zа-яА-Я])(?=.*[0-9])(?=.*[!@#$%^&*()_+]).{8,}$";

        public static string AdminPasswordRegex => @"^(?=.*[a-zA-Zа-яА-Я])(?=.*[0-9])(?=.*[!@#$%^&*()_+]).{12,}$";

        public static Guid UserStatusBlocked => Guid.Parse("a1ac03cf-0686-4a84-909e-9fd03e231d8f");

        public static long PasswordLifeInDays => long.Parse(ConfigurationManager.AppSettings["PasswordLifeInDays"]);

        public static bool UseCaptcha => Convert.ToBoolean(ConfigurationManager.AppSettings["UseCaptcha"]);

        public static string CaptchaSiteKey => ConfigurationManager.AppSettings["CaptchaSiteKey"] ?? string.Empty;

        public static string CaptchaSecretKey => ConfigurationManager.AppSettings["CaptchaSecretKey"] ?? string.Empty;

        public static string GeoNetworkRestApiBaseAddress => ConfigurationManager.AppSettings["GeoNetworkRestApiBaseAddress"] ?? string.Empty;

        public static string GeoNetworkAddress => ConfigurationManager.AppSettings["GeoNetworkAddress"] ?? string.Empty;

        public static string GeoNetworkSearchUrl => $"{GeoNetworkAddress.TrimEnd('/')}/{ConfigurationManager.AppSettings["GeoNetworkSearchUrl"].TrimStart('/')}".Trim();

        public static string GeoNetworkAdminUser => ConfigurationManager.AppSettings["GeoNetworkAdminUser"] ?? string.Empty;

        public static string GeoNetworkAdminPass => ConfigurationManager.AppSettings["GeoNetworkAdminPass"] ?? string.Empty;

        public static string TawtToSource => ConfigurationManager.AppSettings["TawkToSource"] ?? string.Empty;
    }
}