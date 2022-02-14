﻿namespace Moonglade.SEO
{
    public interface ISeoClient
    {
        Task<string?> PostAsync(Uri baseUri, string requestUri, HttpContent content, CancellationToken cancellationToken);

        Task<string?> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken);
    }
}