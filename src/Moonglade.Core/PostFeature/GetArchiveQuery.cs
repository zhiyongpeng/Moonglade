﻿using Moonglade.Data.Spec;
using System.Linq.Expressions;

namespace Moonglade.Core.PostFeature;

public record GetArchiveQuery : IRequest<IReadOnlyList<Archive>>;

public class GetArchiveQueryHandler : IRequestHandler<GetArchiveQuery, IReadOnlyList<Archive>>
{
    private readonly IRepository<PostEntity> _postRepo;
    private readonly Expression<Func<IGrouping<(int Year, int Month), PostEntity>, Archive>> _archiveSelector =
        p => new(p.Key.Year, p.Key.Month, p.Count());

    public GetArchiveQueryHandler(IRepository<PostEntity> postRepo)
    {
        _postRepo = postRepo;
    }

    public async Task<IReadOnlyList<Archive>> Handle(GetArchiveQuery request, CancellationToken cancellationToken)
    {
        if (!_postRepo.Any(p => p.IsPublished && !p.IsDeleted))
        {
            return new List<Archive>();
        }

        var spec = new PostSpec(PostStatus.Published);
        var list = await _postRepo.SelectAsync(
            post => new(post.PubDateUtc.Value.Year, post.PubDateUtc.Value.Month),
            _archiveSelector, spec);

        return list;
    }
}