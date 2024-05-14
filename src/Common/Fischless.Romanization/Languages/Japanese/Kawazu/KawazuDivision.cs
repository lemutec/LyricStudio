using NMeCab.Specialized;
using System.Text;

namespace Fischless.Romanization;

/// <summary>
/// The division from the word separator.
/// </summary>
public class KawazuDivision : List<KawazuJapaneseElement>
{
    public string Surface
    {
        get
        {
            var builder = new StringBuilder();
            foreach (var element in this)
            {
                builder.Append(element.Element);
            }

            return builder.ToString();
        }
    }

    public string HiraReading
    {
        get
        {
            var builder = new StringBuilder();
            foreach (var element in this)
            {
                builder.Append(element.HiraNotation);
            }

            return builder.ToString();
        }
    }

    public string HiraPronunciation
    {
        get
        {
            var builder = new StringBuilder();
            foreach (var element in this)
            {
                builder.Append(element.HiraPronunciation);
            }

            return builder.ToString();
        }
    }

    public string KataReading => KawazuUtilities.ToRawKatakana(HiraReading);
    public string KataPronunciation => KawazuUtilities.ToRawKatakana(HiraPronunciation);
    public string RomaReading => KawazuUtilities.ToRawRomaji(HiraReading);
    public string RomaPronunciation => KawazuUtilities.ToRawRomaji(HiraPronunciation);

    public readonly string PartsOfSpeech;
    public readonly string PartsOfSpeechSection1;
    public readonly string PartsOfSpeechSection2;
    public readonly string PartsOfSpeechSection3;

    public readonly bool IsEndsInTsu;

    public KawazuDivision(MeCabIpaDicNode node, TextType type, RomajiSystem system = RomajiSystem.Hepburn)
    {
        PartsOfSpeech = node.PartsOfSpeech;
        PartsOfSpeechSection1 = node.PartsOfSpeechSection1;
        PartsOfSpeechSection2 = node.PartsOfSpeechSection2;
        PartsOfSpeechSection3 = node.PartsOfSpeechSection3;
        IsEndsInTsu = node.Surface.Last() == 'っ' || node.Surface.Last() == 'ッ';

        switch (type)
        {
            case TextType.PureKana:
                if (node.Surface.Length == node.Pronounciation.Length)
                    for (var i = 0; i < node.Surface.Length; i++)
                        Add(new KawazuJapaneseElement(node.Surface[i].ToString(), KawazuUtilities.ToRawKatakana(node.Surface[i].ToString()), node.Pronounciation[i].ToString(), TextType.PureKana, system));
                else
                    for (var i = 0; i < node.Surface.Length; i++)
                    {
                        var surface = KawazuUtilities.ToRawKatakana(node.Surface[i].ToString());
                        Add(new KawazuJapaneseElement(node.Surface[i].ToString(), surface, surface, TextType.PureKana, system));
                    }
                break;

            case TextType.PureKanji:
                Add(new KawazuJapaneseElement(node.Surface, node.Reading, node.Pronounciation, TextType.PureKanji, system));
                break;

            case TextType.KanjiKanaMixed:
                var surfaceBuilder = new StringBuilder(node.Surface);
                var readingBuilder = new StringBuilder(node.Reading);
                var pronunciationBuilder = new StringBuilder(node.Pronounciation);
                var kanasInTheEnd = new StringBuilder();
                while (KawazuUtilities.IsKana(surfaceBuilder[0])) // Pop the kanas in the front.
                {
                    Add(new KawazuJapaneseElement(surfaceBuilder[0].ToString(), KawazuUtilities.ToRawKatakana(surfaceBuilder[0].ToString()), pronunciationBuilder[0].ToString(), TextType.PureKana, system));
                    surfaceBuilder.Remove(0, 1);
                    readingBuilder.Remove(0, 1);
                    pronunciationBuilder.Remove(0, 1);
                }

                while (KawazuUtilities.IsKana(surfaceBuilder[surfaceBuilder.Length - 1])) // Pop the kanas in the end.
                {
                    kanasInTheEnd.Append(surfaceBuilder[surfaceBuilder.Length - 1].ToString());
                    surfaceBuilder.Remove(surfaceBuilder.Length - 1, 1);
                    readingBuilder.Remove(readingBuilder.Length - 1, 1);
                    pronunciationBuilder.Remove(pronunciationBuilder.Length - 1, 1);
                }

                if (KawazuUtilities.HasKana(surfaceBuilder.ToString())) // For the middle part:
                {
                    var previousIndex = -1;
                    var kanaIndex = 0;

                    var kanas = from ele in surfaceBuilder.ToString()
                                where KawazuUtilities.IsKana(ele) && ele != 'ヶ' && ele != 'ケ'
                                select ele;

                    var kanaList = kanas.ToList();

                    if (kanaList.Count == 0)
                    {
                        Add(new KawazuJapaneseElement(surfaceBuilder.ToString(), readingBuilder.ToString(), pronunciationBuilder.ToString(), TextType.PureKanji, system));
                        break;
                    }

                    foreach (var ch in surfaceBuilder.ToString())
                    {
                        if (KawazuUtilities.IsKanji(ch))
                        {
                            if (kanaIndex >= kanaList.Count)
                            {
                                Add(new KawazuJapaneseElement(ch.ToString(), readingBuilder.ToString(previousIndex + 1, readingBuilder.Length - previousIndex - 1), pronunciationBuilder.ToString(previousIndex + 1, readingBuilder.Length - previousIndex - 1), TextType.PureKanji, system));
                                continue;
                            }

                            var index = readingBuilder.ToString()
                                .IndexOf(KawazuUtilities.ToRawKatakana(kanaList[kanaIndex].ToString()), StringComparison.Ordinal);

                            Add(new KawazuJapaneseElement(ch.ToString(), readingBuilder.ToString(previousIndex + 1, index - previousIndex - 1), pronunciationBuilder.ToString(previousIndex + 1, index - previousIndex - 1), TextType.PureKanji, system));
                            previousIndex = index;
                            kanaIndex++;
                        }

                        if (KawazuUtilities.IsKana(ch))
                        {
                            var kana = KawazuUtilities.ToRawKatakana(ch.ToString());
                            Add(new KawazuJapaneseElement(ch.ToString(), kana, kana, TextType.PureKana, system));
                        }
                    }
                }
                else
                {
                    Add(new KawazuJapaneseElement(surfaceBuilder.ToString(), readingBuilder.ToString(), pronunciationBuilder.ToString(), TextType.PureKanji, system));
                }

                if (kanasInTheEnd.Length != 0)
                {
                    for (var i = kanasInTheEnd.Length - 1; i >= 0; i--)
                    {
                        var kana = KawazuUtilities.ToRawKatakana(kanasInTheEnd.ToString()[i].ToString());
                        Add(new KawazuJapaneseElement(kanasInTheEnd.ToString()[i].ToString(), kana, kana, TextType.PureKana, system));
                    }
                }
                break;

            case TextType.Others:
                Add(new KawazuJapaneseElement(node.Surface, node.Surface, node.Pronounciation, TextType.Others, system));
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}
