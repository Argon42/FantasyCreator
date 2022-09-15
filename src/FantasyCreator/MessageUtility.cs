using System.Text.RegularExpressions;

namespace FantasyCreator;

public static class MessageUtility
{
    private const string MessageKey = "prompt";
    private const string RawMessageKey = "raw_message";

    public static Dictionary<string, string> ParseMessage(string text)
    {
        var dictionary = new Dictionary<string, string>();
        string pattern = "[-—].*?=\\d+(.\\d+)?";
        List<Match> matchesInList = Regex.Matches(text, pattern, RegexOptions.IgnoreCase).ToList();
        string[] matchesInArray = matchesInList.Select(match => match.Groups[0].Value).ToArray();
        foreach (string item in matchesInArray)
        {
            string[] keyAndValue = item.Split('=');
            dictionary.Add(keyAndValue[0].Substring(1, keyAndValue[0].Length - 1), keyAndValue[1].Replace(',', '.'));
        }

        dictionary.Add(MessageKey, Regex.Replace(text, pattern, ""));
        dictionary.Add(RawMessageKey, text);

        return dictionary;
    }
}