using Microsoft.AspNetCore.Components;

public static class MarkdownParser
{
    public static MarkupString ParseSimpleMarkdown(string markdownText)
    {
        // Simple replacements for bold and bullet points
        var html = markdownText
            .Replace("**", "<strong>")
            .Replace("\n-", "<li>")
            .Replace("\n\n", "</li><li>")
            .Replace("\n", "<br />");

        // Close any opened tags
        html = "<p>" + html + "</li></p>";

        return new MarkupString(html);
    }
}
