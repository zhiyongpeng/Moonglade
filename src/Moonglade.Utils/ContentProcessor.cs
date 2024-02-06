﻿using Markdig;
using NUglify;
using System.Text.RegularExpressions;

namespace Moonglade.Utils;

public static class ContentProcessor
{
    public static string ReplaceCDNEndpointToImgTags(this string html, string endpoint)
    {
        if (string.IsNullOrWhiteSpace(html)) return html;

        endpoint = endpoint.TrimEnd('/');
        var imgSrcRegex = new Regex("<img.+?(src)=[\"'](.+?)[\"'].+?>");
        var newStr = imgSrcRegex.Replace(html,
            match => match.Value.Contains("src=\"/image/")
                ? match.Value.Replace("/image/", $"{endpoint}/")
                : match.Value);

        return newStr;
    }

    public static string GetPostAbstract(string content, int wordCount, bool useMarkdown = false)
    {
        var plainText = useMarkdown ?
            MarkdownToContent(content, MarkdownConvertType.Text) :
            RemoveTags(content);

        var result = plainText.Ellipsize(wordCount);
        return result;
    }

    public static string RemoveTags(string html)
    {
        if (string.IsNullOrEmpty(html))
        {
            return string.Empty;
        }

        var result = Uglify.HtmlToText(html);

        return !result.HasErrors && !string.IsNullOrWhiteSpace(result.Code)
            ? result.Code.Trim()
            : RemoveTagsBackup(html);
    }

    public static string Ellipsize(this string text, int characterCount)
    {
        return text.Ellipsize(characterCount, "\u00A0\u2026");
    }

    public static bool IsLetter(this char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z';

    public static bool IsSpace(this char c) => c is '\r' or '\n' or '\t' or '\f' or ' ';

    public static string MarkdownToContent(string markdown, MarkdownConvertType type, bool disableHtml = true)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UsePipeTables()
            .UseBootstrap();

        if (disableHtml)
        {
            pipeline.DisableHtml();
        }

        var result = type switch
        {
            MarkdownConvertType.None => markdown,
            MarkdownConvertType.Html => Markdown.ToHtml(markdown, pipeline.Build()),
            MarkdownConvertType.Text => Markdown.ToPlainText(markdown, pipeline.Build()),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        return result;
    }

    public enum MarkdownConvertType
    {
        None = 0,
        Html = 1,
        Text = 2
    }

    #region Private Methods

    private static string RemoveTagsBackup(string html)
    {
        var result = new char[html.Length];

        var cursor = 0;
        var inside = false;
        foreach (var current in html)
        {
            switch (current)
            {
                case '<':
                    inside = true;
                    continue;
                case '>':
                    inside = false;
                    continue;
            }

            if (!inside)
            {
                result[cursor++] = current;
            }
        }

        var stringResult = new string(result, 0, cursor);

        return stringResult.Replace("&nbsp;", " ");
    }

    private static string Ellipsize(this string text, int characterCount, string ellipsis, bool wordBoundary = false)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";

        if (characterCount < 0 || text.Length <= characterCount)
            return text + ellipsis;

        // search beginning of word
        var backup = characterCount;
        while (characterCount > 0 && text[characterCount - 1].IsLetter())
        {
            characterCount--;
        }

        // search previous word
        while (characterCount > 0 && text[characterCount - 1].IsSpace())
        {
            characterCount--;
        }

        // if it was the last word, recover it, unless boundary is requested
        if (characterCount == 0 && !wordBoundary)
        {
            characterCount = backup;
        }

        var trimmed = text[..characterCount];
        return trimmed + ellipsis;
    }

    #endregion
}