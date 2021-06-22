namespace Inspire.Portal.Services.CaptchaService
{
    using System.Collections.Generic;
    using System.Net;

    using Inspire.Portal.Utilities;
    using Inspire.Utilities.Extensions;

    using Newtonsoft.Json;

    public class ReCaptchaService : ICaptchaService
    {
        public bool Validate(string encodedResponse)
        {
            if (!ConfigurationReader.UseCaptcha)
            {
                return true;
            }

            if (string.IsNullOrEmpty(encodedResponse))
            {
                return false;
            }

            using (var client = new WebClient())
            {
                var result = client.DownloadString(
                    $"https://www.google.com/recaptcha/api/siteverify?secret={ConfigurationReader.CaptchaSecretKey}&response={encodedResponse}");

                var response = JsonConvert.DeserializeObject<CaptchaResponse>(result);

                return response.Success && response.ErrorCodes.IsNullOrEmpty();
            }
        }

        private class CaptchaResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("error-codes")]
            public List<string> ErrorCodes { get; set; }
        }
    }
}