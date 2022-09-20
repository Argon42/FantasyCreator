using System.Text.RegularExpressions;

namespace FantasyCreator;

public static class MessageUtility
{
    private const string MessageKey = "prompt";
    private const string RawMessageKey = "raw_message";

    public static Dictionary<string, string> ParseMessage(string text)
    {
        text = text.Replace("\n", " ").Trim();
        string pattern = "[-—]([\\w_]*?)=([\\S]*)";
        var dictionary = new Dictionary<string, string>();
        foreach (Match match in Regex.Matches(text, pattern, RegexOptions.IgnoreCase))
            if (dictionary.ContainsKey(match.Groups[1].Value) == false)
                dictionary.Add(match.Groups[1].Value, match.Groups[2].Value);
        dictionary.Add(MessageKey, Regex.Replace(text, pattern, ""));
        dictionary.Add(RawMessageKey, text);
        return dictionary;
    }
}