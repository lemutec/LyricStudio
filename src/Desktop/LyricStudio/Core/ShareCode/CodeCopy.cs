using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LyricStudio.Core.ShareCode;

public static class CodeCopy
{
    public static async Task<string> PostCode(string code, string language = "plain_text", string? name = null, DateTime? expireTime = null, string? shareSecret = null, bool preferShareUrl = true)
    {
        if (shareSecret != null && (shareSecret.Length < 4 || shareSecret.Length > 8))
        {
            throw new ArgumentOutOfRangeException(nameof(shareSecret));
        }

        using HttpClient client = new();

        string jsonBody = JsonSerializer.Serialize(new Root()
        {
            theme = "vs-dark",
            codeSegments = new Root.CodeSegments()
            {
                codes =
                [
                    new Root.CodeSegments.Codes()
                    {
                        code = code,
                        language = language,
                        name = name
                    }
                ]
            },
            shareType = 1,
            shareSecret = shareSecret,
            expireTime = expireTime == null ? string.Empty : $"{expireTime:yyyy-MM-ddTHH:mm:ss.fffZ}",
            codeLanguage = "plain_text"
        });
        StringContent reqContent = new(jsonBody, Encoding.UTF8, "application/json");
        HttpResponseMessage res = await client.PostAsync("https://api.codecopy.cn/api/post/add", reqContent);

        if (res.IsSuccessStatusCode)
        {
            string resBody = await res.Content.ReadAsStringAsync();
            var resContent = JsonSerializer.Deserialize<Dictionary<string, object>>(resBody);

            if (resContent["message"].ToString() == "ok")
            {
                if (shareSecret == null)
                {
                    if (preferShareUrl)
                    {
                        return $"https://www.codecopy.cn/post/{resContent["data"]}";
                    }
                    else
                    {
                        return $"https://api.codecopy.cn/api/post/get/vo/code?shareCode={resContent["data"]}";
                    }
                }
                else
                {
                    if (preferShareUrl)
                    {
                        return $"https://www.codecopy.cn/post/{resContent["data"]}?pw={shareSecret}";
                    }
                    else
                    {
                        return $"https://api.codecopy.cn/api/post/get/vo/code?shareCode={resContent["data"]}&shareSecret={shareSecret}";
                    }
                }
            }
        }
        else
        {
            Debug.WriteLine($"Failed to create share, and the detail is '{res.ReasonPhrase}'.");
        }
        return null!;
    }

#pragma warning disable IDE1006

    private class Root
    {
        public class CodeSegments
        {
            public class Codes
            {
                public string code { get; set; }
                public string language { get; set; }
                public string name { get; set; }
            }

            public Codes[] codes { get; set; }
        }

        public string theme { get; set; }
        public CodeSegments codeSegments { get; set; }
        public int shareType { get; set; }
        public string shareSecret { get; set; }
        public string expireTime { get; set; }
        public string codeLanguage { get; set; }
    }

#pragma warning restore IDE1006
}
