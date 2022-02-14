namespace Moonglade.SEO
{
    public class SeoClient : ISeoClient
    {
        private readonly HttpClient _httpClient;

        public SeoClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> PostAsync(Uri baseUri, string requestUri, HttpContent httpContent, CancellationToken cancellationToken)
        {
            _httpClient.BaseAddress = baseUri;
            var httpResponse = await _httpClient.PostAsync(requestUri, httpContent, cancellationToken);
            
            if (httpResponse.IsSuccessStatusCode)
            {
                return await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            }
            else
            {
                return null;
            }
        }

        public async Task<string?> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            var httpResponse = await _httpClient.SendAsync(httpRequestMessage, cancellationToken);

            if (httpResponse.IsSuccessStatusCode)
            {
                return await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            }
            else
            {
                return null;
            }
        }
    }
}
