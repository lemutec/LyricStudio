using System;

namespace LyricStudio.Core.Translators;

public class TranslateException(string? message) : Exception(message)
{
}
