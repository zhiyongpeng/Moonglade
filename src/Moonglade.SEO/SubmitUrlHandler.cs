using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Moonglade.SEO
{
    public class BaiduSubmitUrlHandler : INotificationHandler<SubmitUrlCommand>
    {
        private readonly ILogger<BaiduSubmitUrlHandler> _logger;
        private readonly string? _token;
        private readonly ISeoClient _seoClient;

        public BaiduSubmitUrlHandler(ISeoClient seoClient,
            ILogger<BaiduSubmitUrlHandler> logger,
            IConfiguration configuration)
        {
            _seoClient = seoClient;
            _logger = logger;

            var section = configuration.GetSection("SEO");
            _token = section?.GetValue<string>("BaiduToken", string.Empty);
        }

        public async Task Handle(SubmitUrlCommand notification, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_token))
            {
                return;
            }

            using HttpContent httpContent = new StringContent(notification.PostUrl, Encoding.UTF8);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

            try
            {
                var result = await _seoClient.PostAsync(new Uri("http://data.zz.baidu.com"),
                    $"/urls?site=pzy.io&token={_token}",
                    httpContent, cancellationToken);

                if (result != null)
                {
                    _logger?.LogInformation($"Submit Url to Baidu successfully,response:{result}.");
                }
                else
                {
                    _logger?.LogInformation("Failed to submit Url to Baidu.");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"An exception occurs when submitting Url to Baidu.");
            }
        }
    }

    public class BingSubmitUrlHandler : INotificationHandler<SubmitUrlCommand>
    {
        private readonly ILogger<BingSubmitUrlHandler> _logger;
        private readonly string? _apiKey;
        private readonly ISeoClient _seoClient;

        public BingSubmitUrlHandler(ISeoClient seoClient,
            ILogger<BingSubmitUrlHandler> logger,
            IConfiguration configuration)
        {
            _seoClient = seoClient;
            _logger = logger;

            var section = configuration.GetSection("SEO");
            _apiKey = section?.GetValue<string>("BingToken", string.Empty);
        }

        public async Task Handle(SubmitUrlCommand notification, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return;
            }

            var body = new
            {
                siteUrl = notification.SiteUrl,
                url = notification.PostUrl
            };

            string bingUrl = "https://ssl.bing.com/webmaster/api.svc/json/SubmitUrl?apiKey=" + _apiKey;

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
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"An exception occurs when submitting Url to Bing.");
            }
        }
    }
}
