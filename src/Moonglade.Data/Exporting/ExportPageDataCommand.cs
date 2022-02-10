﻿using MediatR;
using Moonglade.Data.Entities;
using Moonglade.Data.Exporting.Exporters;
using Moonglade.Data.Infrastructure;

namespace Moonglade.Data.Exporting;

public record ExportPageDataCommand : IRequest<ExportResult>;

public class ExportPageDataCommandHandler : IRequestHandler<ExportPageDataCommand, ExportResult>
{
    private readonly IRepository<PageEntity> _pageRepository;

    public ExportPageDataCommandHandler(IRepository<PageEntity> pageRepository)
    {
        _pageRepository = pageRepository;
    }

    public Task<ExportResult> Handle(ExportPageDataCommand request, CancellationToken cancellationToken)
    {
        var pgExp = new ZippedJsonExporter<PageEntity>(_pageRepository, "moonglade-pages", ExportManager.DataDir);
        return pgExp.ExportData(p => new
        {
            p.Id,
            p.Title,
            p.Slug,
            p.MetaDescription,
            p.HtmlContent,
            p.CssContent,
            p.HideSidebar,
            p.IsPublished,
            p.CreateTimeUtc,
            p.UpdateTimeUtc
        }, cancellationToken);
    }
}