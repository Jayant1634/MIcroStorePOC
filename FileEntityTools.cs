using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Free_MS_Store_Apps
{
    internal class FileEntityTools
    {
        public static List<FileEntry> ExtractFileEntries(string html)
        {
            List<FileEntry> fileEntries = new List<FileEntry>();

            MatchCollection matches = Regex.Matches(html, @"<tr[^>]*>(.*?)<\/tr>", RegexOptions.Singleline);
            foreach (Match match in matches)
            {
                string rowHtml = match.Groups[1].Value;
                MatchCollection tdMatches = Regex.Matches(rowHtml, @"<td[^>]*>(.*?)<\/td>", RegexOptions.Singleline);

                if (tdMatches.Count >= 4)
                {
                    string file = StripHtmlTags(tdMatches[0].Groups[1].Value);
                    string expire = StripHtmlTags(tdMatches[1].Groups[1].Value);
                    string sha1 = StripHtmlTags(tdMatches[2].Groups[1].Value);
                    string downloadUrl = ExtractHref(tdMatches[0].Groups[1].Value);
                    string size = StripHtmlTags(tdMatches[3].Groups[1].Value);

                    FileEntry entry = new FileEntry
                    {
                        File = file,
                        Expire = expire,
                        SHA1 = sha1,
                        DownloadUrl = downloadUrl,
                        Size = size
                    };

                    fileEntries.Add(entry);
                }
            }

            return fileEntries;
        }

        static string StripHtmlTags(string input)
        {
            return Regex.Replace(input, "<.*?>", "").Trim();
        }

        static string ExtractHref(string input)
        {
            Match match = Regex.Match(input, @"href=""(.*?)""");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return string.Empty;
        }
    }
}
