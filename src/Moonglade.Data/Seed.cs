﻿using Microsoft.Extensions.Logging;
using Moonglade.Data.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Moonglade.Data;

[ExcludeFromCodeCoverage]
public class Seed
{
    public static async Task SeedAsync(BlogDbContext dbContext, ILogger logger, int retry = 0)
    {
        var retryForAvailability = retry;

        try
        {
            await dbContext.BlogConfiguration.AddRangeAsync(GetBlogConfiguration());
            await dbContext.LocalAccount.AddRangeAsync(GetLocalAccounts());
            await dbContext.BlogTheme.AddRangeAsync(GetThemes());
            await dbContext.Category.AddRangeAsync(GetCategories());
            await dbContext.Tag.AddRangeAsync(GetTags());
            await dbContext.FriendLink.AddRangeAsync(GetFriendLinks());
            await dbContext.Menu.AddRangeAsync(GetMenus());
            await dbContext.CustomPage.AddRangeAsync(GetPages());

            // Add example post
            var content =
                "Moonglade is the new blog system for https://edi.wang. It is a complete rewrite of the old system using .NET 6 and runs on Microsoft Azure.";

            var post = new PostEntity
            {
                Id = Guid.NewGuid(),
                Title = "Welcome to Moonglade",
                Slug = "welcome-to-moonglade",
                Author = "admin",
                PostContent = content,
                CommentEnabled = true,
                CreateTimeUtc = DateTime.UtcNow,
                ContentAbstract = content,
                IsPublished = true,
                IsFeatured = true,
                IsFeedIncluded = true,
                LastModifiedUtc = DateTime.UtcNow,
                PubDateUtc = DateTime.UtcNow,
                ContentLanguageCode = "en-us",
                HashCheckSum = -1688639577,
                IsOriginal = true,
                PostExtension = new()
                {
                    Hits = 1024,
                    Likes = 512
                },
                Tags = dbContext.Tag.ToList(),
                PostCategory = dbContext.PostCategory.ToList()
            };

            await dbContext.Post.AddAsync(post);

            await dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            if (retryForAvailability >= 10) throw;

            retryForAvailability++;

            logger.LogError(e.Message);
            await SeedAsync(dbContext, logger, retryForAvailability);
            throw;
        }
    }

    private static IEnumerable<BlogConfigurationEntity> GetBlogConfiguration()
    {
        return new List<BlogConfigurationEntity>
        {
            new()
            {
                Id = 1,
                CfgKey = "ContentSettings",
                CfgValue = "{\"EnableComments\":true,\"RequireCommentReview\":true,\"EnableWordFilter\":false,\"PostListPageSize\":10,\"HotTagAmount\":10,\"DisharmonyWords\":\"fuck|shit\",\"ShowCalloutSection\":false,\"CalloutSectionHtmlPitch\":\"\"}",
                LastModifiedTimeUtc = DateTime.UtcNow
            },
            new()
            {
                Id = 2,
                CfgKey = "NotificationSettings",
                CfgValue = "{\"EnableEmailSending\":false,\"EnableSsl\":true,\"SendEmailOnCommentReply\":true,\"SendEmailOnNewComment\":true,\"SmtpServerPort\":587,\"AdminEmail\":\"\",\"EmailDisplayName\":\"Moonglade\",\"SmtpPassword\":\"\",\"SmtpServer\":\"\",\"SmtpUserName\":\"\",\"BannedMailDomain\":\"\"}",
                LastModifiedTimeUtc = DateTime.UtcNow
            },
            new()
            {
                Id = 3,
                CfgKey = "FeedSettings",
                CfgValue = "{\"RssItemCount\":20,\"RssCopyright\":\"(c) {year} Moonglade\",\"RssDescription\":\"Latest posts from Moonglade\",\"RssTitle\":\"Moonglade\",\"AuthorName\":\"Admin\",\"UseFullContent\":false}",
                LastModifiedTimeUtc = DateTime.UtcNow
            },
            new()
            {
                Id = 4,
                CfgKey = "GeneralSettings",
                CfgValue = "{\"OwnerName\":\"Admin\",\"OwnerEmail\":\"admin@edi.wang\",\"Description\":\"Moonglade Admin\",\"ShortDescription\":\"Moonglade Admin\",\"AvatarBase64\":\"\",\"SiteTitle\":\"Moonglade\",\"LogoText\":\"moonglade\",\"MetaKeyword\":\"moonglade\",\"MetaDescription\":\"Just another .NET blog system\",\"Copyright\":\"[c] 2022\",\"SideBarCustomizedHtmlPitch\":\"\",\"FooterCustomizedHtmlPitch\":\"\",\"UserTimeZoneBaseUtcOffset\":\"08:00:00\",\"TimeZoneId\":\"China Standard Time\",\"AutoDarkLightTheme\":true,\"ThemeId\":1}",
                LastModifiedTimeUtc = DateTime.UtcNow
            },
            new()
            {
                Id = 5,
                CfgKey = "ImageSettings",
                CfgValue = "{\"IsWatermarkEnabled\":true,\"KeepOriginImage\":false,\"WatermarkFontSize\":20,\"WatermarkText\":\"Moonglade\",\"UseFriendlyNotFoundImage\":true}",
                LastModifiedTimeUtc = DateTime.UtcNow
            },
            new()
            {
                Id = 6,
                CfgKey = "AdvancedSettings",
                CfgValue = "{\"DNSPrefetchEndpoint\":\"\",\"EnableOpenGraph\":true,\"EnablePingBackSend\":true,\"EnablePingBackReceive\":true,\"EnableOpenSearch\":true,\"WarnExternalLink\":true,\"AllowScriptsInPage\":false,\"ShowAdminLoginButton\":false,\"EnablePostRawEndpoint\":true}",
                LastModifiedTimeUtc = DateTime.UtcNow
            },
            new()
            {
                Id = 7,
                CfgKey = "CustomStyleSheetSettings",
                CfgValue = "{\"EnableCustomCss\":false,\"CssCode\":\"\"}",
                LastModifiedTimeUtc = DateTime.UtcNow
            }
        };
    }

    private static IEnumerable<LocalAccountEntity> GetLocalAccounts()
    {
        return new List<LocalAccountEntity>
        {
            new()
            {
                Id = Guid.Parse("ab78493d-7569-42d2-ae78-c2b610ada1aa"),
                Username = "admin",
                PasswordHash = "JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=",
                CreateTimeUtc = DateTime.UtcNow
            }
        };
    }

    private static IEnumerable<BlogThemeEntity> GetThemes()
    {
        return new List<BlogThemeEntity>
        {
            new ()
            {
                ThemeName = "Word Blue", CssRules = "{\"--accent-color1\": \"#2a579a\",\"--accent-color2\": \"#1a365f\",\"--accent-color3\": \"#3e6db5\"}", ThemeType = 0
            },
            new ()
            {
                ThemeName = "Excel Green", CssRules = "{\"--accent-color1\": \"#165331\",\"--accent-color2\": \"#0E351F\",\"--accent-color3\": \"#0E703A\"}", ThemeType = 0
            },
            new ()
            {
                ThemeName = "PowerPoint Orange", CssRules = "{\"--accent-color1\": \"#983B22\",\"--accent-color2\": \"#622616\",\"--accent-color3\": \"#C43E1C\"}", ThemeType = 0
            },
            new ()
            {
                ThemeName = "OneNote Purple", CssRules = "{\"--accent-color1\": \"#663276\",\"--accent-color2\": \"#52285E\",\"--accent-color3\": \"#7719AA\"}", ThemeType = 0
            },
            new ()
            {
                ThemeName = "Outlook Blue", CssRules = "{\"--accent-color1\": \"#035AA6\",\"--accent-color2\": \"#032B51\",\"--accent-color3\": \"#006CBF\"}", ThemeType = 0
            },
            new ()
            {
                ThemeName = "China Red", CssRules = "{\"--accent-color1\": \"#800900\",\"--accent-color2\": \"#5d120d\",\"--accent-color3\": \"#c5170a\"}", ThemeType = 0
            },
            new ()
            {
                ThemeName = "Indian Curry", CssRules = "{\"--accent-color1\": \"rgb(128 84 3)\",\"--accent-color2\": \"rgb(95 62 0)\",\"--accent-color3\": \"rgb(208 142 19)\"}", ThemeType = 0
            },
            new ()
            {
                ThemeName = "Metal Blue", CssRules = "{\"--accent-color1\": \"#4E5967\",\"--accent-color2\": \"#333942\",\"--accent-color3\": \"#6e7c8e\"}", ThemeType = 0
            }
        };
    }

    private static IEnumerable<CategoryEntity> GetCategories()
    {
        return new List<CategoryEntity>
        {
            new()
            {
                Id = Guid.Parse("b0c15707-dfc8-4b09-9aa0-5bfca744c50b"),
                DisplayName = "Default",
                Note = "Default Category",
                RouteName = "default"
            }
        };
    }

    private static IEnumerable<TagEntity> GetTags()
    {
        return new List<TagEntity>
        {
            new() { DisplayName = "Moonglade", NormalizedName = "moonglade" },
            new() { DisplayName = ".NET", NormalizedName = "dot-net" }
        };
    }

    private static IEnumerable<FriendLinkEntity> GetFriendLinks()
    {
        return new List<FriendLinkEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Edi.Wang",
                LinkUrl = "https://edi.wang"
            }
        };
    }

    private static IEnumerable<MenuEntity> GetMenus()
    {
        return new List<MenuEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                DisplayOrder = 0,
                IsOpenInNewTab = false,
                Icon = "icon-star-full",
                Title = "About",
                Url = "/page/about"
            }
        };
    }

    private static IEnumerable<PageEntity> GetPages()
    {
        return new List<PageEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "About",
                Slug = "about",
                MetaDescription = "An Empty About Page",
                HtmlContent = "<h3>An Empty About Page</h3>",
                HideSidebar = true,
                IsPublished = true,
                CreateTimeUtc = DateTime.UtcNow,
                UpdateTimeUtc = DateTime.UtcNow
            }
        };
    }
}