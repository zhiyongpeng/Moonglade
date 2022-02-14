using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Moonglade.SEO
{
    public class BaiduSubmitUrlHandler : INotificationHandler<SubmitUrlCommand>
    {
        private readonly ILogger<BaiduSubmitUrlHandler> _logger;
        private readonly ISeoClient _seoClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;

        public BaiduSubmitUrlHandler(ISeoClient seoClient,
            ILogger<BaiduSubmitUrlHandler> logger,
            IMemoryCache memoryCache,
            IConfiguration configuration)
        {
            _seoClient = seoClient;
            _logger = logger;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        public Task Handle(SubmitUrlCommand notification, CancellationToken cancellationToken)
        {
            var section = _configuration.GetSection("SEO");
            var token = section?.GetValue<string>("BaiduToken", string.Empty);

            if (string.IsNullOrEmpty(token))
            {
                return Task.CompletedTask;
            }

            async Task<string?> SubmitUrl()
            {
                using HttpContent httpContent = new StringContent(notification.PostUrl, Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

                try
                {
                    var result = await _seoClient.PostAsync(new Uri("http://data.zz.baidu.com"),
                        $"/urls?site=pzy.io&token={token}",
                        httpContent, cancellationToken);

                    if (result != null)
                    {
                        _logger?.LogInformation($"Submit Url to Baidu successfully,response:{result}.");
                    }
                    else
                    {
                        _logger?.LogInformation("Failed to submit Url to Baidu.");
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"An exception occurs when submitting Url to Baidu.");
                    return string.Empty;
                }
            }

            return _memoryCache.GetOrCreateAsync($"seo:{notification.PostUrl}",
                async entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromHours(2);
                    return await SubmitUrl();
                });
        }
    }

    public class BingSubmitUrlHandler : INotificationHandler<SubmitUrlCommand>
    {
        private readonly ILogger<BingSubmitUrlHandler> _logger;
        private readonly ISeoClient _seoClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;

        public BingSubmitUrlHandler(ISeoClient seoClient,
            ILogger<BingSubmitUrlHandler> logger,
            IMemoryCache memoryCache,
            IConfiguration configuration)
        {
            _seoClient = seoClient;
            _logger = logger;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        public Task Handle(SubmitUrlCommand notification, CancellationToken cancellationToken)
        {
            var section = _configuration.GetSection("SEO");
            var apiKey = section?.GetValue<string>("BingToken", string.Empty);

            if (string.IsNullOrEmpty(apiKey))
            {
                return Task.CompletedTask;
            }

            async Task<string?> SubmitUrl()
            {
                var body = new
                {
                    siteUrl = notification.SiteUrl,
                    url = notification.PostUrl
                };

                string bingUrl = "https://ssl.bing.com/webmaster/api.svc/json/SubmitUrl?apiKey=" + apiKey;

                try
                {
                    var postRequest = new HttpRequestMessage(HttpMethod.Post, bingUrl)
                    {
                        Content = JsonContent.Create(body)
                    };

                    var result = await _seoClient.SendAsync(postRequest, cancellationToken);

                    if (result != null)
                    {
                        _logger?.LogInformation($"Submit Url to Bing successfully,response:{result}.");
                    }
                    else
                    {
                        _logger?.LogInformation("Failed to submit Url to Bing.");
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"An exception occurs when submitting Url to Bing.");
                    return string.Empty;
                }
            }

            return _memoryCache.GetOrCreateAsync($"seo:{notification.PostUrl}",
                async entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromHours(2);
                    return await SubmitUrl();
                });
        }
    }
}
