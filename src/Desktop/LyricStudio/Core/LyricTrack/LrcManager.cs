using GuiLabs.Undo;
using System.Collections.Generic;
using System.Linq;

namespace LyricStudio.Core.LyricTrack;

public partial class LrcManager
{
    /// <summary>
    /// https://github.com/KirillOsenkov/Undo/blob/main/src/MinimalSample/Program.cs
    /// </summary>
    private readonly ActionManager actionManager = new();

    public List<LrcLine> LrcList { get; private set; } = [];

    public int Count => LrcList.Count;

    public void Clear()
    {
        LrcList.Clear();
    }

    public void LoadText(string text)
    {
        Clear();

        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        LrcList = LrcHelper.ParseText(text).ToList();
    }
}
