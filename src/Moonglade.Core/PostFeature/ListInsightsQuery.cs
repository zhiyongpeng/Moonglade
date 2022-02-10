﻿using Moonglade.Data.Spec;

namespace Moonglade.Core.PostFeature;

public record ListInsightsQuery(PostInsightsType PostInsightsType) : IRequest<IReadOnlyList<PostSegment>>;

public class ListInsightsQueryHandler : IRequestHandler<ListInsightsQuery, IReadOnlyList<PostSegment>>
{
    private readonly IRepository<PostEntity> _postRepo;

    public ListInsightsQueryHandler(IRepository<PostEntity> postRepo)
    {
        _postRepo = postRepo;
    }

    public Task<IReadOnlyList<PostSegment>> Handle(ListInsightsQuery request, CancellationToken cancellationToken)
    {
        var spec = new PostInsightsSpec(request.PostInsightsType, 10);
        return _postRepo.SelectAsync(spec, PostSegment.EntitySelector);
    }
}