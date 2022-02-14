using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Moonglade.SEO
{
    public class BaiduSubmitPostHandler : INotificationHandler<SubmitPostCommand>
    {
        private readonly ILogger<BaiduSubmitPostHandler> _logger;
        private readonly ISeoClient _seoClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;

        public BaiduSubmitPostHandler(ISeoClient seoClient,
            ILogger<BaiduSubmitPostHandler> logger,
            IMemoryCache memoryCache,
            IConfiguration configuration)
        {
            _seoClient = seoClient;
            _logger = logger;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        public async Task Handle(SubmitPostCommand notification, CancellationToken cancellationToken)
        {
            var cacheKey = $"seo:baidu:{notification.PostUrl:N}";
            if (!string.IsNullOrEmpty(_memoryCache.Get<string>(cacheKey)))
            {
                return;
            }

            var section = _configuration.GetSection("SEO");
            var token = section?.GetValue("BaiduToken", string.Empty);

            if (string.IsNullOrEmpty(token))
            {
                return;
            }

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

                _memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromHours(2) });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"An exception occurs when submitting Url to Baidu.");
            }
        }
    }

    public class BingSubmitPostHandler : INotificationHandler<SubmitPostCommand>
    {
        private readonly ILogger<BingSubmitPostHandler> _logger;
        private readonly ISeoClient _seoClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;

        public BingSubmitPostHandler(ISeoClient seoClient,
            ILogger<BingSubmitPostHandler> logger,
            IMemoryCache memoryCache,
            IConfiguration configuration)
        {
            _seoClient = seoClient;
            _logger = logger;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        public async Task Handle(SubmitPostCommand notification, CancellationToken cancellationToken)
        {
            var cacheKey = $"seo:bing:{notification.PostId:N}";
            if (!string.IsNullOrEmpty(_memoryCache.Get<string>(cacheKey)))
            {
                return;
            }

            var section = _configuration.GetSection("SEO");
            var apiKey = section?.GetValue<string>("BingToken", string.Empty);

            if (string.IsNullOrEmpty(apiKey))
            {
                return;
            }

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

                _memoryCache.Set(cacheKey, result, new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromHours(2) });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"An exception occurs when submitting Url to Bing.");
            }
        }
    }
}
