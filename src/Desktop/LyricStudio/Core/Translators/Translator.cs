using LyricStudio.Core.Translators.MicrosoftEdge;
using LyricStudio.Core.Translators.Youdao;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LyricStudio.Core.Translators;

public class Translator
{
    internal HttpClient? _httpClient;
    public TranslatorAPI TranslatorAPI { get; set; } = TranslatorAPI.MicrosoftEdge;

    public string SourceLanguage { get; set; } = "Auto";
    public string TargetLanguage { get; set; } = "Auto";

    public Translator()
    {
        _httpClient = new();
    }

    public async Task<string> TranslateAsync(string text, CancellationToken cancellationToken = default)
    {
        string? sourceLanguage = SourceLanguage;
        string? targetLanguage = TargetLanguage;

        if ("Auto".Equals(sourceLanguage, StringComparison.OrdinalIgnoreCase))
            sourceLanguage = null;
        if ("Auto".Equals(targetLanguage, StringComparison.OrdinalIgnoreCase))
            targetLanguage = null;

        ITranslator translator = TranslatorAPI switch
        {
            TranslatorAPI.Youdao => new YoudaoTranslationQueryResult(_httpClient, sourceLanguage, targetLanguage, text),
            TranslatorAPI.MicrosoftEdge => new EdgeTranslationQueryResult(_httpClient, sourceLanguage, targetLanguage, text),

            _ => throw new Exception("This would never happen")
        };

        return await translator.TranslateAsync(cancellationToken);
    }
}
