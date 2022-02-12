using MediatR;

namespace Moonglade.SEO
{
    /// <summary>
    /// Submit Url Command
    /// </summary>
    public record SubmitUrlCommand(string SiteUrl, string PostUrl) : INotification;
}