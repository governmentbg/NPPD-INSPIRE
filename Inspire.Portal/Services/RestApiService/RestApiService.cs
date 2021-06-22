namespace Inspire.Portal.Services.RestApiService
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Web;

    using Inspire.Core.Infrastructure.Membership;
    using Inspire.Portal.Utilities;
    using Inspire.Utilities.Exception;
    using Inspire.Utilities.Extensions;

    using Newtonsoft.Json.Linq;

    public class RestApiService : IRestApiService
    {
        private readonly IUserPrincipal user;

        public RestApiService(IUserPrincipal user)
        {
            this.user = user;
        }

        public T GetRequest<T>(HttpClient client, string uri)
        {
            var request = CreateRequestMessage(uri);

            var task = client.SendAsync(request);
            task.Wait();

            CheckResultStatusCode(task.Result);

            return task.Result.Content.ReadAsAsync<T>().Result;
        }

        public T PostRequest<T>(HttpClient client, string uri, HttpContent content = null)
        {
            var request = CreateRequestMessage(uri, HttpMethod.Post, content);

            var task = client.SendAsync(request);
            task.Wait();

            CheckResultStatusCode(task.Result);

            return task.Result.Content.ReadAsAsync<T>().Result;
        }

        public void PostRequest(HttpClient client, string uri, HttpContent content = null)
        {
            var request = CreateRequestMessage(uri, HttpMethod.Post, content);

            var task = client.SendAsync(request);
            task.Wait();

            CheckResultStatusCode(task.Result);
        }

        public T PutRequest<T>(HttpClient client, string uri, HttpContent content = null)
        {
            var request = CreateRequestMessage(uri, HttpMethod.Put, content);

            var task = client.SendAsync(request);
            task.Wait();

            CheckResultStatusCode(task.Result);

            return task.Result.Content.ReadAsAsync<T>().Result;
        }

        public void PutRequest(HttpClient client, string uri, HttpContent content = null)
        {
            var request = CreateRequestMessage(uri, HttpMethod.Put, content);

            var task = client.SendAsync(request);
            task.Wait();

            CheckResultStatusCode(task.Result);
        }

        public T PatchRequest<T>(HttpClient client, string uri, HttpContent content = null)
        {
            var request = CreateRequestMessage(uri, new HttpMethod("PATCH"), content);

            var task = client.SendAsync(request);
            task.Wait();

            CheckResultStatusCode(task.Result);

            return task.Result.Content.ReadAsAsync<T>().Result;
        }

        public void PatchRequest(HttpClient client, string uri, HttpContent content = null)
        {
            var request = CreateRequestMessage(uri, new HttpMethod("PATCH"), content);

            var task = client.SendAsync(request);
            task.Wait();

            CheckResultStatusCode(task.Result);
        }

        public T DeleteRequest<T>(HttpClient client, string uri)
        {
            var request = CreateRequestMessage(uri, HttpMethod.Delete);

            var task = client.SendAsync(request);
            task.Wait();

            CheckResultStatusCode(task.Result);

            return task.Result.Content.ReadAsAsync<T>().Result;
        }

        public void DeleteRequest(HttpClient client, string uri)
        {
            var request = CreateRequestMessage(uri, HttpMethod.Delete);

            var task = client.SendAsync(request);
            task.Wait();

            CheckResultStatusCode(task.Result);
        }

        public HttpClient GetClient(IUserPrincipal authorizedUser = null)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(ConfigurationReader.GeoNetworkRestApiBaseAddress)
            };

            InitHeadersData(client, authorizedUser ?? user);

            return client;
        }

        private void CheckResultStatusCode(HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                var result = responseMessage.Content.ReadAsStringAsync().Result;
                var jsonResult = JObject.Parse(result);

                var description = jsonResult.GetValue("description")?.ToString();
                if (description.IsNotNullOrEmptyOrWhiteSpace())
                {
                    throw new UserException(jsonResult.GetValue("description")?.ToString(), new Exception(result));
                }

                throw new HttpException(responseMessage.StatusCode.GetHashCode(), result);
            }
        }

        private HttpRequestMessage CreateRequestMessage(string uri, HttpMethod method = null, HttpContent content = null)
        {
            var httpRequestMessage = CreateBaseRequest(new Uri($"{ConfigurationReader.GeoNetworkRestApiBaseAddress}{uri}"), method);
            if (content != null)
            {
                httpRequestMessage.Content = content;
            }

            return httpRequestMessage;
        }

        private void InitHeadersData(HttpClient client, IUserPrincipal authorizedUser)
        {
            if (authorizedUser?.UserName.IsNotNullOrEmpty() == true && authorizedUser?.Password.IsNotNullOrEmpty() == true)
            {
                client.DefaultRequestHeaders.Add(
                    "Authorization",
                    $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{authorizedUser.UserName}:{authorizedUser.Password}"))}");
            }

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var request = CreateRequestMessage("me", HttpMethod.Get);

            var task = client.SendAsync(request);
            task.Wait();

            CheckResultStatusCode(task.Result);

            var setCookies = task.Result.Headers.Any(item => item.Key == "Set-Cookie")
                ? task.Result.Headers.GetValues("Set-Cookie")
                : null;

            if (setCookies != null && setCookies.Any())
            {
                var token = setCookies.First(item => item.StartsWith("XSRF-TOKEN", StringComparison.InvariantCultureIgnoreCase)).Split(';').First().Split('=').Last();
                var sessionId = setCookies.FirstOrDefault(item => item.StartsWith("JSESSIONID", StringComparison.InvariantCultureIgnoreCase))?.Split(';').First().Split('=').Last();

                client.DefaultRequestHeaders.Add("X-XSRF-TOKEN", token);
                client.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={token};JSESSIONID={sessionId};");
            }
        }

        private HttpRequestMessage CreateBaseRequest(Uri uri, HttpMethod method = null)
        {
            return new HttpRequestMessage(method ?? HttpMethod.Get, uri);
        }
    }
}