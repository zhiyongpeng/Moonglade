using MediatR;

namespace Moonglade.SEO
{
    /// <summary>
    /// Submit Url Command
    /// </summary>
    public record SubmitPostCommand(
                Guid PostId,
                string SiteUrl,
                string PostUrl) : INotification;
}