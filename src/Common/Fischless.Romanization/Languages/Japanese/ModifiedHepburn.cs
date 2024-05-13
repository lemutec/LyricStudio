using Fischless.Romanization.Internal;
using System.Diagnostics.Contracts;

namespace Romanization;

public static partial class Japanese
{
    /// <summary>
    /// The Modified Hepburn Japanese romanization system.<br />
    /// For more information, visit:
    /// <a href='https://en.wikipedia.org/wiki/Hepburn_romanization' />
    /// </summary>
    public sealed class ModifiedHepburn
    {
        // System-Specific Constants
        private readonly Dictionary<string, string> GojuonChart = [];

        private readonly Dictionary<string, string> YoonChart = [];

        private readonly CharSub LongASub = new($"a{Choonpu}", Constants.MacronA, false);
        private readonly CharSub LongESub = new($"e{Choonpu}", Constants.MacronE, false);
        private readonly CharSub LongISub = new($"i{Choonpu}", Constants.MacronI, false);
        private readonly CharSub LongOSub = new($"o{Choonpu}", Constants.MacronO, false);
        private readonly CharSub LongUSub = new($"u{Choonpu}", Constants.MacronU, false);

        private readonly CharSub SyllabicNVowelsSub = new($"[{SyllabicNHiragana}{SyllabicNKatakana}]([{Constants.LatinVowels}])", "n'${1}");
        private readonly CharSub SyllabicNConsonantsSub = new($"[{SyllabicNHiragana}{SyllabicNKatakana}]([{Constants.LatinConsonants}])", "n${1}");

        private readonly CharSub SokuonGeneralCaseSub = new($"[{SokuonHiragana}{SokuonKatakana}]([{Constants.LatinConsonants}])", "${1}${1}", false);
        private readonly CharSub SokuonChCaseSub = new($"[{SokuonHiragana}{SokuonKatakana}]ch", "tch", false);

        /// <summary>
        /// Instantiates a copy of the system to process romanizations.
        /// </summary>
        public ModifiedHepburn()
        {
            // Sourced from https://en.wikipedia.org/wiki/Hepburn_romanization#Romanization_charts
            // Each pair is hiragana, then katakana, respectively

            // Gojūon
            // Column 1
            GojuonChart["あ"] = "a";
            GojuonChart["ア"] = "a";
            GojuonChart["か"] = "ka";
            GojuonChart["カ"] = "ka";
            GojuonChart["さ"] = "sa";
            GojuonChart["サ"] = "sa";
            GojuonChart["た"] = "ta";
            GojuonChart["タ"] = "ta";
            GojuonChart["な"] = "na";
            GojuonChart["ナ"] = "na";
            GojuonChart["は"] = "ha";
            GojuonChart["ハ"] = "ha";
            GojuonChart["ま"] = "ma";
            GojuonChart["マ"] = "ma";
            GojuonChart["や"] = "ya";
            GojuonChart["ヤ"] = "ya";
            GojuonChart["ら"] = "ra";
            GojuonChart["ラ"] = "ra";
            GojuonChart["わ"] = "wa";
            GojuonChart["ワ"] = "wa";
            GojuonChart["が"] = "ga";
            GojuonChart["ガ"] = "ga";
            GojuonChart["ざ"] = "za";
            GojuonChart["ザ"] = "za";
            GojuonChart["だ"] = "da";
            GojuonChart["ダ"] = "da";
            GojuonChart["ば"] = "ba";
            GojuonChart["バ"] = "ba";
            GojuonChart["ぱ"] = "pa";
            GojuonChart["パ"] = "pa";
            // Column 2
            GojuonChart["い"] = "i";
            GojuonChart["イ"] = "i";
            GojuonChart["き"] = "ki";
            GojuonChart["キ"] = "ki";
            GojuonChart["し"] = "shi";
            GojuonChart["シ"] = "shi";
            GojuonChart["ち"] = "chi";
            GojuonChart["チ"] = "chi";
            GojuonChart["に"] = "ni";
            GojuonChart["ニ"] = "ni";
            GojuonChart["ひ"] = "hi";
            GojuonChart["ヒ"] = "hi";
            GojuonChart["み"] = "mi";
            GojuonChart["ミ"] = "mi";
            GojuonChart["り"] = "ri";
            GojuonChart["リ"] = "ri";
            GojuonChart["ゐ"] = "i"; // historical character
            GojuonChart["ヰ"] = "i"; // historical character
            GojuonChart["ぎ"] = "gi";
            GojuonChart["ギ"] = "gi";
            GojuonChart["じ"] = "ji";
            GojuonChart["ジ"] = "ji";
            GojuonChart["ぢ"] = "ji";
            GojuonChart["ヂ"] = "ji";
            GojuonChart["び"] = "bi";
            GojuonChart["ビ"] = "bi";
            GojuonChart["ぴ"] = "pi";
            GojuonChart["ピ"] = "pi";
            // Column 3
            GojuonChart["う"] = "u";
            GojuonChart["ウ"] = "u";
            GojuonChart["く"] = "ku";
            GojuonChart["ク"] = "ku";
            GojuonChart["す"] = "su";
            GojuonChart["ス"] = "su";
            GojuonChart["つ"] = "tsu";
            GojuonChart["ツ"] = "tsu";
            GojuonChart["ぬ"] = "nu";
            GojuonChart["ヌ"] = "nu";
            GojuonChart["ふ"] = "fu";
            GojuonChart["フ"] = "fu";
            GojuonChart["む"] = "mu";
            GojuonChart["ム"] = "mu";
            GojuonChart["ゆ"] = "yu";
            GojuonChart["ユ"] = "yu";
            GojuonChart["る"] = "ru";
            GojuonChart["ル"] = "ru";
            GojuonChart["ぐ"] = "gu";
            GojuonChart["グ"] = "gu";
            GojuonChart["ず"] = "zu";
            GojuonChart["ズ"] = "zu";
            GojuonChart["づ"] = "zu";
            GojuonChart["ヅ"] = "zu";
            GojuonChart["ぶ"] = "bu";
            GojuonChart["ブ"] = "bu";
            GojuonChart["ぷ"] = "pu";
            GojuonChart["プ"] = "pu";
            // Column 4
            GojuonChart["え"] = "e";
            GojuonChart["エ"] = "e";
            GojuonChart["け"] = "ke";
            GojuonChart["ケ"] = "ke";
            GojuonChart["せ"] = "se";
            GojuonChart["セ"] = "se";
            GojuonChart["て"] = "te";
            GojuonChart["テ"] = "te";
            GojuonChart["ね"] = "ne";
            GojuonChart["ネ"] = "ne";
            GojuonChart["へ"] = "he";
            GojuonChart["ヘ"] = "he";
            GojuonChart["め"] = "me";
            GojuonChart["メ"] = "me";
            GojuonChart["れ"] = "re";
            GojuonChart["レ"] = "re";
            GojuonChart["ゑ"] = "e"; // historical character
            GojuonChart["ヱ"] = "e"; // historical character
            GojuonChart["げ"] = "ge";
            GojuonChart["ゲ"] = "ge";
            GojuonChart["ぜ"] = "ze";
            GojuonChart["ゼ"] = "ze";
            GojuonChart["で"] = "de";
            GojuonChart["デ"] = "de";
            GojuonChart["べ"] = "be";
            GojuonChart["ベ"] = "be";
            GojuonChart["ぺ"] = "pe";
            GojuonChart["ペ"] = "pe";
            // Column 5
            GojuonChart["お"] = "o";
            GojuonChart["オ"] = "o";
            GojuonChart["こ"] = "ko";
            GojuonChart["コ"] = "ko";
            GojuonChart["そ"] = "so";
            GojuonChart["ソ"] = "so";
            GojuonChart["と"] = "to";
            GojuonChart["ト"] = "to";
            GojuonChart["の"] = "no";
            GojuonChart["ノ"] = "no";
            GojuonChart["ほ"] = "ho";
            GojuonChart["ホ"] = "ho";
            GojuonChart["も"] = "mo";
            GojuonChart["モ"] = "mo";
            GojuonChart["よ"] = "yo";
            GojuonChart["ヨ"] = "yo";
            GojuonChart["ろ"] = "ro";
            GojuonChart["ロ"] = "ro";
            GojuonChart["を"] = "o"; // rare particle
            GojuonChart["ヲ"] = "o"; // rare particle
            GojuonChart["ん"] = "n";
            GojuonChart["ン"] = "n";
            GojuonChart["ご"] = "go";
            GojuonChart["ゴ"] = "go";
            GojuonChart["ぞ"] = "zo";
            GojuonChart["ゾ"] = "zo";
            GojuonChart["ど"] = "do";
            GojuonChart["ド"] = "do";
            GojuonChart["ぼ"] = "bo";
            GojuonChart["ボ"] = "bo";
            GojuonChart["ぽ"] = "po";
            GojuonChart["ポ"] = "po";

            // Yōon
            // Column 1
            YoonChart["きゃ"] = "kya";
            YoonChart["キャ"] = "kya";
            YoonChart["しゃ"] = "sha";
            YoonChart["シャ"] = "sha";
            YoonChart["ちゃ"] = "cha";
            YoonChart["チャ"] = "cha";
            YoonChart["にゃ"] = "nya";
            YoonChart["ニャ"] = "nya";
            YoonChart["ひゃ"] = "hya";
            YoonChart["ヒャ"] = "hya";
            YoonChart["みゃ"] = "mya";
            YoonChart["ミャ"] = "mya";
            YoonChart["りゃ"] = "rya";
            YoonChart["リャ"] = "rya";
            YoonChart["ぎゃ"] = "gya";
            YoonChart["ギャ"] = "gya";
            YoonChart["じゃ"] = "ja";
            YoonChart["ジャ"] = "ja";
            YoonChart["ぢゃ"] = "ja";
            YoonChart["ヂャ"] = "ja";
            YoonChart["びゃ"] = "bya";
            YoonChart["ビャ"] = "bya";
            YoonChart["ぴゃ"] = "pya";
            YoonChart["ピャ"] = "pya";
            // Column 2
            YoonChart["きゅ"] = "kyu";
            YoonChart["キュ"] = "kyu";
            YoonChart["しゅ"] = "shu";
            YoonChart["シュ"] = "shu";
            YoonChart["ちゅ"] = "chu";
            YoonChart["チュ"] = "chu";
            YoonChart["にゅ"] = "nyu";
            YoonChart["ニュ"] = "nyu";
            YoonChart["ひゅ"] = "hyu";
            YoonChart["ヒュ"] = "hyu";
            YoonChart["みゅ"] = "myu";
            YoonChart["ミュ"] = "myu";
            YoonChart["りゅ"] = "ryu";
            YoonChart["リュ"] = "ryu";
            YoonChart["ぎゅ"] = "gyu";
            YoonChart["ギュ"] = "gyu";
            YoonChart["じゅ"] = "ju";
            YoonChart["ジュ"] = "ju";
            YoonChart["ぢゅ"] = "ju";
            YoonChart["ヂュ"] = "ju";
            YoonChart["びゅ"] = "byu";
            YoonChart["ビュ"] = "byu";
            YoonChart["ぴゅ"] = "pyu";
            YoonChart["ピュ"] = "pyu";
            // Column 3
            YoonChart["きょ"] = "kyo";
            YoonChart["キョ"] = "kyo";
            YoonChart["しょ"] = "sho";
            YoonChart["ショ"] = "sho";
            YoonChart["ちょ"] = "cho";
            YoonChart["チョ"] = "cho";
            YoonChart["にょ"] = "nyo";
            YoonChart["ニョ"] = "nyo";
            YoonChart["ひょ"] = "hyo";
            YoonChart["ヒョ"] = "hyo";
            YoonChart["みょ"] = "myo";
            YoonChart["ミョ"] = "myo";
            YoonChart["りょ"] = "ryo";
            YoonChart["リョ"] = "ryo";
            YoonChart["ぎょ"] = "gyo";
            YoonChart["ギョ"] = "gyo";
            YoonChart["じょ"] = "jo";
            YoonChart["ジョ"] = "jo";
            YoonChart["ぢょ"] = "jo";
            YoonChart["ヂョ"] = "jo";
            YoonChart["びょ"] = "byo";
            YoonChart["ビョ"] = "byo";
            YoonChart["ぴょ"] = "pyo";
            YoonChart["ピョ"] = "pyo";
            // Column 4
            YoonChart["ヴァ"] = "va";
            YoonChart["ヴィ"] = "vi";
            YoonChart["ヴェ"] = "ve";
            YoonChart["ヴォ"] = "vo";
            YoonChart["ヴン"] = "vn";
            YoonChart["ディ"] = "di";
            YoonChart["ティ"] = "ti";
        }

        /// <summary>
        /// Performs Modified Hepburn romanization on the given text.<br />
        /// Note this supports Hiragana and Katakana, but not Kanji.<br />
        /// Due to limits of machine romanization, certain details of the system are left out - namely vowel
        /// combinations, as the rules for combining vowels depend on whether or not the vowels share a
        /// morpheme (something not known to the program).
        /// </summary>
        /// <param name="text">The text to romanize.</param>
        /// <returns>A romanized version of the text, leaving unrecognized characters untouched. Note that all romanized text will be lowercase.</returns>
        [Pure]
        public string Process(string text)
            => text
                // Replace common alternate characters
                .ReplaceCommonAlternates()
                // Insert spaces at boundaries between Latin characters and Japanese ones (ie. ニンテンドーDSiブラウザー)
                .SeparateLanguageBoundaries()
                // Do multi-char combinations first (Yōon)
                .ReplaceFromChartWithSameCase(YoonChart, StringComparison.CurrentCulture)
                // Then single-char replacements (Gojūon)
                .ReplaceFromChartWithSameCase(GojuonChart, StringComparison.CurrentCulture)
                // Do special subsitutions
                .ReplaceMany(
                    // Convert chōonpu usage in original text into macrons to mark long vowels in a romanized manner
                    LongASub, LongESub, LongISub, LongOSub, LongUSub,
                    // Render syllabic n as either "n'" or "n" based on whether or not it preceeds a vowel or consonant, respectively
                    SyllabicNVowelsSub, SyllabicNConsonantsSub,
                    // Take sokuon usage into account (repeating the following consonant to mark long consonants)
                    SokuonChCaseSub, SokuonGeneralCaseSub);
    }
}
