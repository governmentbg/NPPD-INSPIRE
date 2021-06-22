namespace Inspire.Portal.Services.RestApiService
{
    using System.Net.Http;

    using Inspire.Core.Infrastructure.Membership;

    public interface IRestApiService
    {
        T GetRequest<T>(HttpClient client, string uri);

        T PostRequest<T>(HttpClient client, string uri, HttpContent content = null);

        void PostRequest(HttpClient client, string uri, HttpContent content = null);

        T PutRequest<T>(HttpClient client, string uri, HttpContent content = null);

        void PutRequest(HttpClient client, string uri, HttpContent content = null);

        T PatchRequest<T>(HttpClient client, string uri, HttpContent content = null);

        void PatchRequest(HttpClient client, string uri, HttpContent content = null);

        T DeleteRequest<T>(HttpClient client, string uri);

        void DeleteRequest(HttpClient client, string uri);

        HttpClient GetClient(IUserPrincipal authorizedUser = null);
    }
}