using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using URandom = UnityEngine.Random;

public class Utils
{
    public static NormalItem.eNormalType GetRandomNormalType()
    {
        Array values = Enum.GetValues(typeof(NormalItem.eNormalType));
        NormalItem.eNormalType result = (NormalItem.eNormalType)values.GetValue(URandom.Range(0, values.Length));

        return result;
    }

    public static NormalItem.eNormalType GetRandomNormalTypeExcept(NormalItem.eNormalType[] types)
    {
        List<NormalItem.eNormalType> list = Enum.GetValues(typeof(NormalItem.eNormalType)).Cast<NormalItem.eNormalType>().Except(types).ToList();

        int rnd = URandom.Range(0, list.Count);
        NormalItem.eNormalType result = list[rnd];

        return result;
    }

    /// <summary>
    /// Strips HTML tags and comments from input text, keeping only TextMeshPro supported rich text tags.
    /// </summary>
    public static string CleanHtmlTags(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";

        // 1. Remove comments: <!-- ... -->
        string cleaned = Regex.Replace(input, @"<!--.*?-->", "", RegexOptions.Singleline);

        // 2. Replace block level tags with newlines to preserve formatting
        cleaned = Regex.Replace(cleaned, @"<(br|br\s*/)>", "\n", RegexOptions.IgnoreCase);
        cleaned = Regex.Replace(cleaned, @"</?(div|p|h1|h2|h3|h4|h5|h6|li|ul|ol)>", "\n", RegexOptions.IgnoreCase);

        // 3. Remove all other HTML tags EXCEPT standard TextMeshPro rich text tags
        string whitelist = "b|i|u|s|color|size|align|font|sprite|link|style|noparse|page|pos|space|cspace|mark|sub|sup|voffset|width|br";
        string pattern = @"<(?!/?(" + whitelist + @")\b)[^>]+>";
        cleaned = Regex.Replace(cleaned, pattern, "", RegexOptions.IgnoreCase);

        // 4. Decode HTML entities
        cleaned = cleaned.Replace("&nbsp;", " ")
                         .Replace("&lt;", "<")
                         .Replace("&gt;", ">")
                         .Replace("&amp;", "&")
                         .Replace("&quot;", "\"")
                         .Replace("&apos;", "'");

        // 5. Clean up extra leading/trailing newlines
        cleaned = cleaned.Trim();

        // 6. Replace multiple consecutive newlines (more than 2) with just 2 newlines for cleaner display
        cleaned = Regex.Replace(cleaned, @"\n{3,}", "\n\n");

        return cleaned;
    }
}
