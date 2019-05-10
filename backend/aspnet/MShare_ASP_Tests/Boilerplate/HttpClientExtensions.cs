using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace MShare_ASP_Tests.Boilerplate {
    public static class HttpClientExtensions {
        public static async Task<HttpResponseMessage> PostJsonAsync<T>(this HttpClient client, string route, T data) {
            var formatter = new JsonMediaTypeFormatter();
            return await client.PostAsync(route, new ObjectContent(typeof(T), data, formatter));
        }
    }
}
