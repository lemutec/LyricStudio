using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LyricStudio.Core.Translators.Youdao;

public class YoudaoTranslationQueryResult : ITranslator
{
    private readonly HttpClient _httpClient;
    private readonly string? _sourceLanguage;
    private readonly string? _targetLanguage;

    public string Title => "Translate text";

    public string Description => "Translate specified text with 'Youdao'";

    public string Text { get; }

    public YoudaoTranslationQueryResult(HttpClient httpClient, string? sourceLanguage, string? targetLanguage, string text)
    {
        _httpClient = httpClient;
        _sourceLanguage = sourceLanguage;
        _targetLanguage = targetLanguage;
        Text = text;
    }

    public async Task<string?> TranslateAsync(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://aidemo.youdao.com/trans"),
            Content = new FormUrlEncodedContent(
                new Dictionary<string, string>()
                {
                    ["from"] = _sourceLanguage ?? "auto",
                    ["to"] = _targetLanguage ?? "auto",
                    ["q"] = Text
                })
        };

        var response = await _httpClient.SendAsync(request, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<YoudaoApiResult>(Converter.Settings, cancellationToken);

        var resultText = result?.Translation?.FirstOrDefault();
        if (!string.IsNullOrEmpty(resultText))
        {
            return resultText;
        }
        return null;
    }
}
