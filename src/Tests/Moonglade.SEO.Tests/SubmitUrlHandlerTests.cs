using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moonglade.SEO.Tests
{
    [TestFixture]
    public class SubmitUrlHandlerTests
    {
        private MockRepository _mockRepository;
        private Mock<ISeoClient> _mockSeoClient;
        private IConfiguration _configuration;

        private readonly Dictionary<string, string> _appSettingsStub = new Dictionary<string, string> {
            {"SEO:BaiduToken", "BaiduToken"},
            {"SEO:BingToken", "BingToken"}
};

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new(MockBehavior.Default);
            _mockSeoClient = _mockRepository.Create<ISeoClient>();
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(_appSettingsStub)
                .Build();
        }

        [Test]
        public async Task Test_Baidu_SubmitUrl_OK()
        {
            var mockLogger = _mockRepository.Create<ILogger<BaiduSubmitUrlHandler>>();

            var handler =
                new BaiduSubmitUrlHandler(_mockSeoClient.Object, mockLogger.Object, _configuration);

            await handler.Handle(
                new SubmitUrlCommand("https://pzy.io/", "https://pzy.io/post/2022/1/23/highperf-aop-aspectinjector-tutorial"),
                default);
        }

        [Test]
        public async Task Test_Bing_SubmitUrl_OK()
        {
            var mockLogger = _mockRepository.Create<ILogger<BingSubmitUrlHandler>>();

            var handler =
                new BingSubmitUrlHandler(_mockSeoClient.Object, mockLogger.Object, _configuration);

            await handler.Handle(
                new SubmitUrlCommand("https://pzy.io/", "https://pzy.io/post/2022/1/23/highperf-aop-aspectinjector-tutorial"),
                default);
        }
    }
}
