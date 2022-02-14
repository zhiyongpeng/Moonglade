using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Moonglade.SEO.Tests
{
    [TestFixture]
    public class SubmitUrlHandlerTests
    {
        private MockRepository _mockRepository;
        private ISeoClient _mockSeoClient;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new(MockBehavior.Default);
            var httpClient = new HttpClient();

            _mockSeoClient = new SeoClient(httpClient);
        }

        [Test]
        public async Task Test_Baidu_SubmitUrl_OK()
        {
            var mockLogger = _mockRepository.Create<ILogger<BaiduSubmitUrlHandler>>();

            var _appSettingsStub = new Dictionary<string, string> {
            {"SEO:BaiduToken", "BaiduToken"}};

            var configuration = new ConfigurationBuilder()
               .AddInMemoryCollection(_appSettingsStub)
               .Build();

            var handler =
                new BaiduSubmitUrlHandler(_mockSeoClient, mockLogger.Object, configuration);

            await handler.Handle(
                new SubmitUrlCommand("https://pzy.io", "https://pzy.io/post/2022/1/23/highperf-aop-aspectinjector-tutorial"),
                default);
        }

        [Test]
        public async Task Test_Bing_SubmitUrl_OK()
        {
            var mockLogger = _mockRepository.Create<ILogger<BingSubmitUrlHandler>>();

            var _appSettingsStub = new Dictionary<string, string> {
            {"SEO:BingToken", "BaiduToken"}};

            var configuration = new ConfigurationBuilder()
               .AddInMemoryCollection(_appSettingsStub)
               .Build();

            var handler =
                new BingSubmitUrlHandler(_mockSeoClient, mockLogger.Object, configuration);

            await handler.Handle(
                new SubmitUrlCommand("https://pzy.io", "https://pzy.io/post/2022/1/23/highperf-aop-aspectinjector-tutorial"),
                default);
        }

        [Test]
        public async Task Test_Baidu_SubmitUrl_WithoutToken_OK()
        {
            var mockLogger = _mockRepository.Create<ILogger<BaiduSubmitUrlHandler>>();

            var mockConfiguration = _mockRepository.Create<IConfiguration>();

            var handler =
                new BaiduSubmitUrlHandler(_mockSeoClient, mockLogger.Object, mockConfiguration.Object);

            await handler.Handle(
                new SubmitUrlCommand("https://pzy.io", "https://pzy.io/post/2022/1/23/highperf-aop-aspectinjector-tutorial"),
                default);
        }

        [Test]
        public async Task Test_Bing_SubmitUrl_WithoutToken_OK()
        {
            var mockLogger = _mockRepository.Create<ILogger<BingSubmitUrlHandler>>();

            var mockConfiguration = _mockRepository.Create<IConfiguration>();

            var handler =
                new BingSubmitUrlHandler(_mockSeoClient, mockLogger.Object, mockConfiguration.Object);

            await handler.Handle(
                new SubmitUrlCommand("https://pzy.io", "https://pzy.io/post/2022/1/23/highperf-aop-aspectinjector-tutorial"),
                default);
        }
    }
}
