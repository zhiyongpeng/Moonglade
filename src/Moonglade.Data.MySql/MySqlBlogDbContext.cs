﻿using Microsoft.EntityFrameworkCore;
using Moonglade.Data.MySql.Configurations;
using System.Diagnostics.CodeAnalysis;

namespace Moonglade.Data.MySql;

[ExcludeFromCodeCoverage]
public class MySqlBlogDbContext : BlogDbContext
{
    public MySqlBlogDbContext()
    {
    }

    public MySqlBlogDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CommentConfiguration());
        modelBuilder.ApplyConfiguration(new CommentReplyConfiguration());
        modelBuilder.ApplyConfiguration(new PostConfiguration());
        modelBuilder.ApplyConfiguration(new PostCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new PostExtensionConfiguration());
        modelBuilder.ApplyConfiguration(new LocalAccountConfiguration());
        modelBuilder.ApplyConfiguration(new PingbackConfiguration());
        modelBuilder.ApplyConfiguration(new BlogThemeConfiguration());
        modelBuilder.ApplyConfiguration(new BlogAssetConfiguration());
        modelBuilder.ApplyConfiguration(new BlogConfigurationConfiguration());
        modelBuilder.ApplyConfiguration(new PageConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}