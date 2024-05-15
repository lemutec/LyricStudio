using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Fischless.Design.Controls;
using Fischless.Linq;
using Fischless.Linq.Collections;
using Fischless.Mapper;
using Fischless.Mvvm;
using Fischless.Romanization;
using Fischless.Win32;
using Fischless.Win32.SystemDialog;
using LyricStudio.Core.AudioTrack;
using LyricStudio.Core.Configuration;
using LyricStudio.Core.LyricTrack;
using LyricStudio.Core.MusicTag;
using LyricStudio.Core.Player;
using LyricStudio.Core.ShareCode;
using LyricStudio.Core.Translators;
using LyricStudio.Models;
using LyricStudio.Models.Audios;
using LyricStudio.Models.Messages;
using LyricStudio.Services;
using LyricStudio.Views;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace LyricStudio.ViewModels;

public partial class HomePageViewModel : ObservableObject, IDisposable
{
    [ObservableProperty]
    private LyricEditMode mode = LyricEditMode.ListView;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsMediaAvailable))]
    private IAudioPlayer audioPlayer = new FFMEAudioPlayer();

    private readonly Task timer = null!;
    private CancellationTokenSource timerTokenSource = null!;

    private readonly LrcManager lrcManager = new();

    [ObservableProperty]
    private ObservableCollectionEx<ObservableLrcLine> lrcLines = [];

    /// <summary>
    /// Used for <see cref="LyricEditMode.ListView"/>
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EditinglrcLine))]
    private ObservableLrcLine selectedlrcLine = null!;

    partial void OnSelectedlrcLineChanged(ObservableLrcLine value)
    {
        EditinglrcLine = value?.ToString();
    }

    /// <summary>
    /// Used for <see cref="LyricEditMode.ListView"/>
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LrcLines))]
    private string editinglrcLine = null!;

    partial void OnEditinglrcLineChanged(string value)
    {
        if (SelectedlrcLine != null)
        {
            LrcLine v = LrcLine.Parse(value);

            SelectedlrcLine.LrcText = v.LrcText;
            SelectedlrcLine.LrcTime = v.LrcTime;

            if (Mode == LyricEditMode.ListView)
            {
                IsSaved = false;
            }
        }
    }

    /// <summary>
    /// Display the current lyric in progress bar
    /// Used for <see cref="LyricEditMode.ListView"/>
    /// </summary>
    [ObservableProperty]
    private string currentLrcText = string.Empty;

    /// <summary>
    /// Used for <see cref="LyricEditMode.TextBox"/>
    /// TODO: Fix the not right with this property
    /// </summary>
    [ObservableProperty]
    private string lyricText = null!;

    partial void OnLyricTextChanged(string? oldValue, string newValue)
    {
        if (Mode == LyricEditMode.TextBox)
        {
            if (oldValue == null)
            {
                // Ignore the first loading
                return;
            }

            IsSaved = false;
        }
    }

    [ObservableProperty]
    private bool isPlaying = false;

    [ObservableProperty]
    private double currentTime = default;

    [ObservableProperty]
    private double totalTime = default;

    [ObservableProperty]
    private double position = default;

    [ObservableProperty]
    private double volume = ConfigurationKeys.Volume.Get();

    partial void OnVolumeChanged(double value)
    {
        ConfigurationKeys.Volume.Set(value);
        Config.Configer?.Save(AppConfig.SettingsFile);
        if (IsMediaAvailable)
        {
            AudioPlayer.Volume = (int)(value * 100d);
        }
    }

    [ObservableProperty]
    private double rate = 1d;

    partial void OnRateChanged(double value)
    {
        if (IsMediaAvailable)
        {
            AudioPlayer.Rate = value;
        }
    }

    [ObservableProperty]
    private string tagName = string.Empty;

    [ObservableProperty]
    private string tagArtist = string.Empty;

    [ObservableProperty]
    private string tagAlbumName = string.Empty;

    [ObservableProperty]
    private byte[] tagAlbumImage = null!;

    [ObservableProperty]
    private string lyricFilePath = null!;

    [ObservableProperty]
    private string musicFilePath = null!;

    [ObservableProperty]
    private int tagDuration = 0;

    [ObservableProperty]
    private string tagBitRate = string.Empty;

    [ObservableProperty]
    private List<AudioVolume> volumes = [];

    [ObservableProperty]
    private bool isSaved = true;

    partial void OnIsSavedChanged(bool value)
    {
        SyncWindowTitle();
    }

    [ObservableProperty]
    private bool isReading = false;

    protected bool IsMediaAvailable => AudioPlayer != null;

    [ObservableProperty]
    private bool isShowAudioVisualization = ConfigurationKeys.IsShowAudioVisualization.Get();

    partial void OnIsShowAudioVisualizationChanged(bool value)
    {
        ConfigurationKeys.IsShowAudioVisualization.Set(value);
        Config.Configer?.Save(AppConfig.SettingsFile);
    }

    [ObservableProperty]
    private bool isShowHighting = ConfigurationKeys.IsShowHighting.Get();

    partial void OnIsShowHightingChanged(bool value)
    {
        ConfigurationKeys.IsShowHighting.Set(value);
        Config.Configer?.Save(AppConfig.SettingsFile);

        if (!value)
        {
            (LrcLines as IEnumerable<ObservableLrcLine>).ForEach(v => v.IsHightlight = false);
        }
    }

    [Obsolete("It feels worthless")]
    [ObservableProperty]
    private bool isUseTwoDigitTimeCode = ConfigurationKeys.IsUseTwoDigitTimeCode.Get();

    partial void OnIsUseTwoDigitTimeCodeChanged(bool value)
    {
        ConfigurationKeys.IsUseTwoDigitTimeCode.Set(value);
        Config.Configer?.Save(AppConfig.SettingsFile);
    }

    [ObservableProperty]
    private int flagTimeOffset = ConfigurationKeys.FlagTimeOffset.Get();

    partial void OnFlagTimeOffsetChanged(int value)
    {
        ConfigurationKeys.FlagTimeOffset.Set(value);
        Config.Configer?.Save(AppConfig.SettingsFile);
    }

    [ObservableProperty]
    private double shortShiftSeconds = ConfigurationKeys.ShortShiftSeconds.Get();

    partial void OnShortShiftSecondsChanged(double value)
    {
        ConfigurationKeys.ShortShiftSeconds.Set(value);
        Config.Configer?.Save(AppConfig.SettingsFile);
    }

    [ObservableProperty]
    private double longShiftSeconds = ConfigurationKeys.LongShiftSeconds.Get();

    partial void OnLongShiftSecondsChanged(double value)
    {
        ConfigurationKeys.LongShiftSeconds.Set(value);
        Config.Configer?.Save(AppConfig.SettingsFile);
    }

    [SupportedOSPlatform("Windows")]
    public HomePageViewModel()
    {
        timerTokenSource = new();
        timer = Task.Factory.StartNew(() =>
        {
            while (!timerTokenSource.Token.IsCancellationRequested)
            {
                OnTick();
                Thread.Sleep(30);
            }
        }, TaskCreationOptions.LongRunning);

        WeakReferenceMessenger.Default.Register<FileDropMessage>(this, async (sender, msg) =>
        {
            await OpenFilesAsync(msg.FileNames);
        });
    }

    [SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize")]
    public void Dispose()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
        timerTokenSource?.Cancel();
    }

    private void OnTick()
    {
        if (!IsMediaAvailable)
        {
            return;
        }
        SyncCurrentLyricAndHightlight();
    }

    private void SyncCurrentLyricAndHightlight()
    {
        if (IsShowHighting)
        {
            LrcLine line = LrcHelper.GetNearestLrc(LrcLines, TimeSpan.FromSeconds(CurrentTime));

            CurrentLrcText = line?.LrcText ?? string.Empty;

            (LrcLines as IEnumerable<ObservableLrcLine>).ForEach(v => v.IsHightlight = false);
            if (line is ObservableLrcLine oLine)
            {
                oLine.IsHightlight = true;
            }
        }
        else
        {
            if (Mode == LyricEditMode.ListView)
            {
                LrcLine line = LrcHelper.GetNearestLrc(LrcLines, TimeSpan.FromSeconds(CurrentTime));
                CurrentLrcText = line?.LrcText ?? string.Empty;
            }
        }
    }

    [SupportedOSPlatform("Windows")]
    [SuppressMessage("Style", "IDE0074:Use compound assignment")]
    private async Task OpenFilesAsync(params string[] fileNames)
    {
        IsReading = true;
        Stop();

        try
        {
            string? lyricFile = fileNames.Where(f => !string.IsNullOrWhiteSpace(f) && LrcHelper.LyricExtensions.Contains(Path.GetExtension(f).ToLower())).FirstOrDefault();
            string? musicFile = fileNames.Where(f => MediaInfoAudio.HasAudioTrack(f)).FirstOrDefault();

            if (lyricFile == null)
            {
                lyricFile = fileNames.Where(f => !string.IsNullOrWhiteSpace(f) && (new FileInfo(f).Extension?.Equals(".ass", StringComparison.OrdinalIgnoreCase) ?? false)).FirstOrDefault();
            }

            if (lyricFile == null && musicFile == null)
            {
                return;
            }

            // Auto load lyric/subtitle/text file
            if (lyricFile == null)
            {
                bool autoLoadLrc = false;

                if (Mode == LyricEditMode.ListView)
                {
                    if (LrcLines.IsEmpty())
                    {
                        autoLoadLrc = true;
                    }
                }
                else if (Mode == LyricEditMode.TextBox)
                {
                    if (string.IsNullOrWhiteSpace(LyricText))
                    {
                        autoLoadLrc = true;
                    }
                }

                if (autoLoadLrc)
                {
                    string? lyricFileHatena = Path.ChangeExtension(musicFile, "lrc");

                    if (File.Exists(lyricFileHatena))
                    {
                        lyricFile = lyricFileHatena;
                    }
                    else
                    {
                        lyricFileHatena = Path.ChangeExtension(musicFile, "ass");

                        if (File.Exists(lyricFileHatena))
                        {
                            lyricFile = lyricFileHatena;
                        }
                        else
                        {
                            lyricFileHatena = Path.ChangeExtension(musicFile, "txt");

                            if (File.Exists(lyricFileHatena))
                            {
                                lyricFile = lyricFileHatena;
                            }
                        }
                    }
                }
            }

            if (lyricFile != null)
            {
                IsSaved = true;

                if (Path.GetExtension(lyricFile).Equals(".ass", StringComparison.OrdinalIgnoreCase))
                {
                    if (Ass2lrc.AssToLyric(LrcHelper.ReadAllText(lyricFile), out string lyricText))
                    {
                        lyricFile = Path.ChangeExtension(lyricFile, "lrc");

                        LyricText = lyricText;
                        IsSaved = false;
                    }
                    else
                    {
                        lyricFile = null;
                    }
                }
                else
                {
                    LyricText = LrcHelper.ReadAllText(lyricFile);
                }

                LyricFilePath = lyricFile;
                lrcManager.LoadText(LyricText);
                LrcLines.Reset(lrcManager.LrcList.Select(v => MapperProvider.Map<LrcLine, ObservableLrcLine>(v)));
            }

            if (musicFile == null)
            {
                string? musicFileHatena = Directory.GetFiles(Path.GetDirectoryName(lyricFile), $"{Path.GetFileNameWithoutExtension(lyricFile)}.*")
                    .Where(f => !f.EndsWith(".lrc", StringComparison.OrdinalIgnoreCase) && !f.EndsWith(".aas", StringComparison.OrdinalIgnoreCase))
                    .Where(MediaInfoAudio.HasAudioTrack)
                    .FirstOrDefault();

                if (musicFileHatena != null)
                {
                    musicFile = musicFileHatena;
                }
            }

            if (musicFile != null)
            {
                MusicFilePath = musicFile;

                if (string.IsNullOrWhiteSpace(LyricFilePath))
                {
                    LyricFilePath = Path.ChangeExtension(MusicFilePath, "lrc");
                }

                TagAlbumImage = null!;
                MusicInfo musicInfo = await Task.Run(() =>
                {
                    MusicInfoLoaderByTagLib musicInfoLoader = new();
                    MusicInfo musicInfo = musicInfoLoader.Load(musicFile);

                    if (musicInfo.Status != MusicInfoStatus.MusicTagInvalid)
                    {
                        musicInfo.BitRate = MediaInfoAudio.GetAudioBitRate(musicFile) / 1000d;
                        musicInfo.Volumes = AudioInfoProvider.GetAudioVolume(musicFile).ToList();
                        musicInfo.TotalTime = AudioInfoProvider.GetTotalTime(musicFile);
                    }
                    return musicInfo;
                });

                if (musicInfo.Status != MusicInfoStatus.MusicTagInvalid)
                {
                    TagAlbumName = musicInfo.AlbumName;
                    TagName = musicInfo.Name;
                    TagArtist = musicInfo.Artist;
                    TagAlbumImage = musicInfo.AlbumImage;
                    TagDuration = musicInfo.Duration;
                    TagBitRate = $"{musicInfo.BitRate} kbps";
                    Volumes = musicInfo.Volumes;
                    TotalTime = musicInfo.TotalTime;
                }

                AudioPlayer.Open(musicFile);
                AudioPlayer.PositionChanged += OnPositionChanged;
            }

            if (!string.IsNullOrWhiteSpace(LyricFilePath))
            {
                LyricFilePath = Path.ChangeExtension(LyricFilePath, "lrc");
            }

            if (!File.Exists(LyricFilePath))
            {
                IsSaved = false;
            }

            SyncWindowTitle();
        }
        catch (Exception e)
        {
            _ = e;
        }
        finally
        {
            IsReading = false;
        }
    }

    private void SyncWindowTitle()
    {
        if (string.IsNullOrWhiteSpace(MusicFilePath) || string.IsNullOrWhiteSpace(LyricFilePath))
        {
            WeakReferenceMessenger.Default.Send(new GlobalMessage(this, GlobalCommand.ChangeMainWindowTitle, string.Empty));
            return;
        }

        string fileName = LyricFilePath;

        if (!IsSaved)
        {
            fileName = $"{fileName}•";
        }

        WeakReferenceMessenger.Default.Send(new GlobalMessage(this, GlobalCommand.ChangeMainWindowTitle, fileName));
    }

    [RelayCommand]
    public void PlayOrPause()
    {
        if (!IsMediaAvailable)
        {
            return;
        }

        switch (AudioPlayer.State)
        {
            case AudioPlayerState.Buffering:
            case AudioPlayerState.Opening:
            case AudioPlayerState.Error:
                break;

            case AudioPlayerState.Playing:
            case AudioPlayerState.Ended:
                IsPlaying = false;
                AudioPlayer.Pause();
                break;

            case AudioPlayerState.None:
            case AudioPlayerState.Paused:
            case AudioPlayerState.Stopped:
                IsPlaying = true;
                AudioPlayer.Volume = (int)(Volume * 100d);
                AudioPlayer.Rate = Rate;
                AudioPlayer.Play();
                break;
        }
    }

    [RelayCommand]
    public void Play()
    {
        IsPlaying = true;
        AudioPlayer.Volume = (int)(Volume * 100d);
        AudioPlayer.Rate = Rate;
        AudioPlayer.Play();
    }

    [RelayCommand]
    public void Stop()
    {
        if (!IsMediaAvailable)
        {
            return;
        }

        CurrentTime = default;
        Position = default;
        IsPlaying = false;
        AudioPlayer.Stop();
    }

    [RelayCommand]
    public void ShortShiftLeft()
    {
        SeekInSeconds(CurrentTime - ConfigurationKeys.ShortShiftSeconds.Get());
    }

    [RelayCommand]
    public void ShortShiftRight()
    {
        SeekInSeconds(CurrentTime + ConfigurationKeys.ShortShiftSeconds.Get());
    }

    [RelayCommand]
    public void LongShiftLeft()
    {
        SeekInSeconds(CurrentTime - ConfigurationKeys.LongShiftSeconds.Get());
    }

    [RelayCommand]
    public void LongShiftRight()
    {
        SeekInSeconds(CurrentTime + ConfigurationKeys.LongShiftSeconds.Get());
    }

    public void SeekInSeconds(double second)
    {
        if (!IsMediaAvailable)
        {
            return;
        }

        AudioPlayer.SeekTo(second);
    }

    public void SeekInPosition(double position)
    {
        SeekInSeconds(position * TotalTime);
    }

    private void OnPositionChanged(object? sender, double position)
    {
        double currentTime4update = position * TotalTime;

        // Time offset >= 0.5s
        if (Math.Abs(CurrentTime - currentTime4update) >= 0.5d)
        {
            CurrentTime = currentTime4update;
        }

        // Position offset >= 0.25%
        if (Math.Abs(Position - position) >= 0.0025d)
        {
            Position = position;
        }
    }

    [RelayCommand]
    public void Flag()
    {
        if (!IsMediaAvailable)
        {
            return;
        }

        if (Mode != LyricEditMode.ListView)
        {
            return;
        }

        if (SelectedlrcLine == null)
        {
            if (LrcLines.Any())
            {
                ObservableLrcLine firstTake = LrcLines.Where(l => l.LrcTime.HasValue).FirstOrDefault();

                if (firstTake != null)
                {
                    SelectedlrcLine = firstTake;
                }
            }
        }

        if (SelectedlrcLine != null)
        {
            if (SelectedlrcLine.LrcTime.HasValue)
            {
                SelectedlrcLine.LrcTime = TimeSpan.FromSeconds(AudioPlayer.Position)
                    .Add(TimeSpan.FromMicroseconds(ConfigurationKeys.FlagTimeOffset.Get()));
                EditinglrcLine = SelectedlrcLine.ToString();
                IsSaved = false;

                // Select next line
                int nextIndex = LrcLines.IndexOf(SelectedlrcLine) + 1;
                if (nextIndex <= LrcLines.Count - 1)
                {
                    SelectedlrcLine = LrcLines[nextIndex];
                    EditinglrcLine = SelectedlrcLine.ToString();
                }
            }
        }
    }

    /// <summary>
    /// Switch mode
    /// </summary>
    [RelayCommand]
    public void Exchange()
    {
        LyricEditMode targetMode = Mode switch
        {
            LyricEditMode.ListView => LyricEditMode.TextBox,
            LyricEditMode.TextBox => LyricEditMode.ListView,
            _ => throw new NotImplementedException(),
        };

        if (targetMode == LyricEditMode.ListView)
        {
            lrcManager.LoadText(LyricText);
            LrcLines.Reset(lrcManager.LrcList.Select(v => MapperProvider.Map<LrcLine, ObservableLrcLine>(v)));
        }
        else if (targetMode == LyricEditMode.TextBox)
        {
            LyricText = string.Join(Environment.NewLine, LrcLines.Select(l => l.ToString()));
        }

        Mode = targetMode;
    }

    [RelayCommand]
    [SupportedOSPlatform("Windows")]
    [SupportedOSPlatform("MacOS")]
    public async Task OpenAudioFile()
    {
        if (await new OpenFileDialog() { Title = "打开音乐文件" }.ShowAsync() is OpenFileDialogResult result)
        {
            await OpenFilesAsync(result.Item.FileInfo.FullName);
        }
    }

    [RelayCommand]
    [SupportedOSPlatform("Windows")]
    [SupportedOSPlatform("MacOS")]
    public async Task OpenLyricFile()
    {
        if (await new OpenFileDialog()
        {
            Title = "打开歌词文件",
            Filters = [
                new FileDialogFilter()
                {
                    Name = "歌词文件",
                    Extensions = [.. LrcHelper.LyricExtensions.Select(ext => ext.Replace(".", string.Empty))]
                },
            ]
        }.ShowAsync() is OpenFileDialogResult result)
        {
            await OpenFilesAsync(result.Item.FileInfo.FullName);
        }
    }

    [RelayCommand]
    public void SaveLyricFile()
    {
        try
        {
            if (Mode == LyricEditMode.ListView)
            {
                File.WriteAllLines(LyricFilePath, LrcLines.Select(l => l.ToString()));
            }
            else if (Mode == LyricEditMode.TextBox)
            {
                File.WriteAllText(LyricFilePath, LyricText ?? string.Empty);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            Log.Warning(e.ToString());
        }
        IsSaved = true;
    }

    [RelayCommand]
    [SupportedOSPlatform("Windows")]
    [SupportedOSPlatform("MacOS")]
    public async Task SaveAsLyricFile()
    {
        SaveFileDialogResult? result = await new SaveFileDialog()
        {
            Title = "保存歌词文件",
            InitialFileName = Path.GetFileName(LyricFilePath),
            DefaultExtension = ".lrc",
            Filters = [
                new FileDialogFilter()
                {
                    Name = "歌词文件",
                    Extensions = ["lrc"]
                }
            ]
        }.ShowAsync();

        if (result == null)
        {
            return;
        }

        try
        {
            if (Mode == LyricEditMode.ListView)
            {
                File.WriteAllLines(result.FileInfo.FullName, LrcLines.Select(l => l.ToString()));
            }
            else if (Mode == LyricEditMode.TextBox)
            {
                File.WriteAllText(result.FileInfo.FullName, LyricText ?? string.Empty);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            Log.Warning(e.ToString());
        }
    }

    [RelayCommand]
    [SupportedOSPlatform("Windows")]
    [SupportedOSPlatform("MacOS")]
    public async Task SaveAsTextFile()
    {
        SaveFileDialogResult? result = await new SaveFileDialog()
        {
            Title = "保存文本文件",
            InitialFileName = Path.ChangeExtension(Path.GetFileName(LyricFilePath), "txt"),
            DefaultExtension = ".txt",
            Filters = [
                new FileDialogFilter()
                {
                    Name = "文本文件",
                    Extensions = ["txt"]
                }
            ]
        }.ShowAsync();

        if (result == null)
        {
            return;
        }

        try
        {
            if (Mode == LyricEditMode.ListView)
            {
                string lrc = string.Join(Environment.NewLine, LrcLines.Select(l => l.ToString()));
                File.WriteAllText(result.FileInfo.FullName, LrcHelper.StripTimeMark(lrc));
            }
            else if (Mode == LyricEditMode.TextBox)
            {
                string lrc = LyricText ?? string.Empty;
                File.WriteAllText(result.FileInfo.FullName, LrcHelper.StripTimeMark(lrc));
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            Log.Warning(e.ToString());
        }
    }

    [RelayCommand]
    [SupportedOSPlatform("Windows")]
    [SupportedOSPlatform("MacOS")]
    public async Task SaveAsAssFile()
    {
        SaveFileDialogResult? result = await new SaveFileDialog()
        {
            Title = "保存字幕文件",
            InitialFileName = Path.ChangeExtension(Path.GetFileName(LyricFilePath), "ass"),
            DefaultExtension = ".ass",
            Filters = [
                new FileDialogFilter()
                {
                    Name = "字幕文件",
                    Extensions = ["ass"]
                }
            ]
        }.ShowAsync();

        if (result == null)
        {
            return;
        }

        try
        {
            if (Mode == LyricEditMode.ListView)
            {
                string lrc = string.Join(Environment.NewLine, LrcLines.Select(l => l.ToString()));

                if (Lrc2Ass.AssToLyric(lrc, out string ass))
                {
                    File.WriteAllText(result.FileInfo.FullName, ass);
                }
            }
            else if (Mode == LyricEditMode.TextBox)
            {
                string lrc = LyricText ?? string.Empty;

                if (Lrc2Ass.AssToLyric(lrc, out string ass))
                {
                    File.WriteAllText(result.FileInfo.FullName, ass);
                }
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            Log.Warning(e.ToString());
        }
    }

    [RelayCommand]
    public async Task CopyLyric()
    {
        if (Mode == LyricEditMode.ListView)
        {
            string? text = string.Join(Environment.NewLine, LrcLines.Select(l => l.ToString()));

            await App.GetService<IClipboardService>()
                .SetTextAsync(text ?? string.Empty);
        }
        else if (Mode == LyricEditMode.TextBox)
        {
            await App.GetService<IClipboardService>()
                .SetTextAsync(LyricText ?? string.Empty);
        }
    }

    [RelayCommand]
    public void CloseFiles()
    {
        LyricFilePath = null!;
        MusicFilePath = null!;

        // Close backend classes
        if (IsMediaAvailable)
        {
            Stop();
        }
        lrcManager.Clear();

        // Close view model classes
        LrcLines.Clear();
        TagAlbumImage = null!;
        Volumes = [];
        SelectedlrcLine = null!;
        CurrentLrcText = string.Empty;
        EditinglrcLine = null!;
        LyricText = string.Empty;
        CurrentTime = default;
        TotalTime = default;
        Position = default;
        IsSaved = true;
        SyncWindowTitle();
    }

    [RelayCommand]
    public void LineInsertPrev()
    {
        if (SelectedlrcLine == null)
        {
            return;
        }

        if (Mode != LyricEditMode.ListView)
        {
            return;
        }

        int index = LrcLines.IndexOf(SelectedlrcLine);

        if (index - 1 < 0)
        {
            index = 0;
        }

        LrcLines.Insert(index, new ObservableLrcLine()
        {
            LrcTime = TimeSpan.Zero,
            LrcText = string.Empty,
        });
        SelectedlrcLine = LrcLines[index];
    }

    [RelayCommand]
    public void LineInsertNext()
    {
        if (SelectedlrcLine == null)
        {
            return;
        }

        if (Mode != LyricEditMode.ListView)
        {
            return;
        }

        int index = LrcLines.IndexOf(SelectedlrcLine);

        index++;
        if (index > LrcLines.Count)
        {
            index = LrcLines.Count;
        }

        LrcLines.Insert(index, new ObservableLrcLine()
        {
            LrcTime = TimeSpan.Zero,
            LrcText = string.Empty,
        });
        SelectedlrcLine = LrcLines[index];
    }

    [RelayCommand]
    public void ContinuousMarkPrev()
    {
        if (SelectedlrcLine == null)
        {
            return;
        }

        if (Mode != LyricEditMode.ListView)
        {
            return;
        }

        int index = LrcLines.IndexOf(SelectedlrcLine);

        if (index - 1 < 0)
        {
            return;
        }

        var prevLine = LrcLines[index - 1];
        if (SelectedlrcLine.LrcTime.HasValue && prevLine.LrcTime.HasValue)
        {
            SelectedlrcLine.LrcTime = prevLine.LrcTime;
            EditinglrcLine = SelectedlrcLine.ToString();
        }
    }

    [RelayCommand]
    public void ContinuousMarkNext()
    {
        if (SelectedlrcLine == null)
        {
            return;
        }

        if (Mode != LyricEditMode.ListView)
        {
            return;
        }

        int index = LrcLines.IndexOf(SelectedlrcLine);

        if (index + 1 >= LrcLines.Count)
        {
            return;
        }

        var nextLine = LrcLines[index - 1];
        if (SelectedlrcLine.LrcTime.HasValue && nextLine.LrcTime.HasValue)
        {
            SelectedlrcLine.LrcTime = LrcLines[index + 1].LrcTime;
            EditinglrcLine = SelectedlrcLine.ToString();
        }
    }

    [RelayCommand]
    public void ShiftTimecodeMinus200()
    {
        if (SelectedlrcLine == null)
        {
            return;
        }

        if (Mode != LyricEditMode.ListView)
        {
            return;
        }

        if (SelectedlrcLine.LrcTime.HasValue)
        {
            SelectedlrcLine.LrcTime += TimeSpan.FromSeconds(-0.2d);
            if (SelectedlrcLine.LrcTime < TimeSpan.Zero)
            {
                SelectedlrcLine.LrcTime = TimeSpan.Zero;
            }
            EditinglrcLine = SelectedlrcLine.ToString();
        }
    }

    [RelayCommand]
    public void ShiftTimecodePlus200()
    {
        if (SelectedlrcLine == null)
        {
            return;
        }

        if (Mode != LyricEditMode.ListView)
        {
            return;
        }

        if (SelectedlrcLine.LrcTime.HasValue)
        {
            SelectedlrcLine.LrcTime += TimeSpan.FromSeconds(0.2d);
            EditinglrcLine = SelectedlrcLine.ToString();
        }
    }

    [RelayCommand]
    public async Task CutLine()
    {
        if (SelectedlrcLine == null)
        {
            return;
        }

        if (Mode != LyricEditMode.ListView)
        {
            return;
        }

        await App.GetService<IClipboardService>()
            .SetTextAsync(SelectedlrcLine.ToString() ?? string.Empty);

        int index = LrcLines.IndexOf(SelectedlrcLine);
        LrcLines.RemoveAt(index);
    }

    [RelayCommand]
    public async Task CopyLine()
    {
        if (SelectedlrcLine == null)
        {
            return;
        }

        if (Mode != LyricEditMode.ListView)
        {
            return;
        }

        await App.GetService<IClipboardService>()
            .SetTextAsync(SelectedlrcLine.ToString() ?? string.Empty);
    }

    [RelayCommand]
    public async Task PasteLine()
    {
        if (SelectedlrcLine == null)
        {
            return;
        }

        if (Mode != LyricEditMode.ListView)
        {
            return;
        }

        string? text = await App.GetService<IClipboardService>().GetTextAsync();

        if (text != null)
        {
            IEnumerable<LrcLine> lines = LrcHelper.ParseText(text);

            if (lines.Any())
            {
                LrcLine line = lines.First();

                if (line.LrcTime.HasValue)
                {
                    SelectedlrcLine.LrcTime = line.LrcTime;
                }
                SelectedlrcLine.LrcText = line.LrcText;
            }
        }
    }

    [RelayCommand]
    public void RepeatLine()
    {
        if (SelectedlrcLine == null)
        {
            return;
        }

        if (Mode != LyricEditMode.ListView)
        {
            return;
        }

        int index = LrcLines.IndexOf(SelectedlrcLine);

        index++;
        if (index > LrcLines.Count)
        {
            index = LrcLines.Count;
        }

        LrcLines.Insert(index, new ObservableLrcLine()
        {
            LrcTime = SelectedlrcLine.LrcTime,
            LrcText = SelectedlrcLine.LrcText,
        });
    }

    [RelayCommand]
    public void ResetCurrentTimeMark()
    {
        if (SelectedlrcLine == null)
        {
            return;
        }

        if (Mode != LyricEditMode.ListView)
        {
            return;
        }

        if (SelectedlrcLine.LrcTime.HasValue)
        {
            SelectedlrcLine.LrcTime = TimeSpan.Zero;
            EditinglrcLine = SelectedlrcLine.ToString();
        }
    }

    [RelayCommand]
    public async Task Undo()
    {
        await MessageBox.WarnAsync(new NotSupportedException().Message, "Undo");
    }

    [RelayCommand]
    public async Task Redo()
    {
        await MessageBox.WarnAsync(new NotSupportedException().Message, "Redo");
    }

    [RelayCommand]
    public void MoveUpLine()
    {
        if (SelectedlrcLine == null)
        {
            return;
        }

        if (Mode != LyricEditMode.ListView)
        {
            return;
        }

        int index = LrcLines.IndexOf(SelectedlrcLine);

        if (index - 1 < 0)
        {
            return;
        }

        LrcLines.Move(index, index - 1);
        SelectedlrcLine = LrcLines[index - 1];
    }

    [RelayCommand]
    public void MoveDownLine()
    {
        if (SelectedlrcLine == null)
        {
            return;
        }

        if (Mode != LyricEditMode.ListView)
        {
            return;
        }

        int index = LrcLines.IndexOf(SelectedlrcLine);

        if (index + 1 >= LrcLines.Count)
        {
            return;
        }

        LrcLines.Move(index, index + 1);
        SelectedlrcLine = LrcLines[index + 1];
    }

    [RelayCommand]
    public void RemoveLine()
    {
        if (SelectedlrcLine == null)
        {
            return;
        }

        if (Mode != LyricEditMode.ListView)
        {
            return;
        }

        int index = LrcLines.IndexOf(SelectedlrcLine);
        LrcLines.RemoveAt(index);
    }

    /// <summary>
    /// Caused by double tapped
    /// </summary>
    [RelayCommand]
    public void PlaySeekLyric()
    {
        if (SelectedlrcLine is null)
        {
            return;
        }

        if (SelectedlrcLine.LrcTime.HasValue)
        {
            SeekInSeconds(SelectedlrcLine.LrcTime.Value.TotalSeconds);
        }
    }

    /// <summary>
    /// Change to [00:00.00]
    /// </summary>
    [RelayCommand]
    public void ResetTimeMark()
    {
        if (Mode == LyricEditMode.ListView)
        {
            (LrcLines as IEnumerable<ObservableLrcLine>)
                .ForEach(line =>
                {
                    if (line.LrcTime.HasValue)
                    {
                        line.LrcTime = TimeSpan.Zero;
                    }
                });
        }
        else if (Mode == LyricEditMode.TextBox)
        {
            LyricText = LrcHelper.ParseText(LyricText)
                .ForEach(line =>
                {
                    if (line.LrcTime.HasValue)
                    {
                        line.LrcTime = TimeSpan.Zero;
                    }
                })
                .Select(line => line.ToString())
                .Aggregate((current, next) => current + Environment.NewLine + next);
        }
    }

    [RelayCommand]
    public async Task ShiftTimecode()
    {
        ShiftTimecodeDialog dialog = App.GetService<ShiftTimecodeDialog>();
        double? shiftTime = await dialog.GetShiftTimeAsync();

        if (shiftTime == null || shiftTime == 0d)
        {
            return;
        }

        TimeSpan offset = TimeSpan.FromSeconds(shiftTime.Value);

        if (Mode == LyricEditMode.ListView)
        {
            (LrcLines as IEnumerable<ObservableLrcLine>)
                .ForEach(line =>
                {
                    if (line.LrcTime.HasValue)
                    {
                        line.LrcTime += offset;
                        if (line.LrcTime < TimeSpan.Zero)
                        {
                            line.LrcTime = TimeSpan.Zero;
                        }
                    }
                });
        }
        else if (Mode == LyricEditMode.TextBox)
        {
            LyricText = LrcHelper.ParseText(LyricText)
                .ForEach(line =>
                {
                    if (line.LrcTime.HasValue)
                    {
                        line.LrcTime += offset;
                        if (line.LrcTime < TimeSpan.Zero)
                        {
                            line.LrcTime = TimeSpan.Zero;
                        }
                    }
                })
                .Select(line => line.ToString())
                .Aggregate((current, next) => current + Environment.NewLine + next);
        }
    }

    [RelayCommand]
    public async Task Translate()
    {
        Translator translator = new();

        if (Mode == LyricEditMode.ListView)
        {
            foreach (ObservableLrcLine line in LrcLines)
            {
                if (!line.LrcTime.HasValue)
                {
                    continue;
                }

                string? translated = await translator.TranslateAsync(line.LrcText);
                line.LrcText = $"{line.LrcText}  {translated}";
            }
        }
        else if (Mode == LyricEditMode.TextBox)
        {
            IEnumerable<LrcLine> lrcLines = LrcHelper.ParseText(LyricText ?? string.Empty);

            foreach (LrcLine line in lrcLines)
            {
                if (!line.LrcTime.HasValue)
                {
                    continue;
                }

                string? translated = await translator.TranslateAsync(line.LrcText);
                line.LrcText = $"{line.LrcText}  {translated}";
            }

            LyricText = string.Join(Environment.NewLine, lrcLines.Select(l => l.ToString()));
        }
    }

    [RelayCommand]
    public void TranslateToTraditional()
    {
        if (Mode == LyricEditMode.ListView)
        {
            foreach (ObservableLrcLine line in LrcLines)
            {
                if (!line.LrcTime.HasValue)
                {
                    continue;
                }

                line.LrcText = ChineseConverter.ToTraditional(line.LrcText);
            }
        }
        else if (Mode == LyricEditMode.TextBox)
        {
            LyricText = ChineseConverter.ToTraditional(LyricText);
        }
    }

    [RelayCommand]
    public void TranslateToSimplified()
    {
        if (Mode == LyricEditMode.ListView)
        {
            foreach (ObservableLrcLine line in LrcLines)
            {
                if (!line.LrcTime.HasValue)
                {
                    continue;
                }

                line.LrcText = ChineseConverter.ToSimplified(line.LrcText);
            }
        }
        else if (Mode == LyricEditMode.TextBox)
        {
            LyricText = ChineseConverter.ToSimplified(LyricText);
        }
    }

    [RelayCommand]
    public async Task TranslateKanjiToRomaji()
    {
        RomanizationDialog dialog = App.GetService<RomanizationDialog>();
        IRomanizationDialogSettings? settings = await dialog.GetRomanizationSettingsAsync();

        if (settings == null)
        {
            return;
        }

        try
        {
            if (Mode == LyricEditMode.ListView)
            {
                foreach (ObservableLrcLine line in LrcLines)
                {
                    if (!line.LrcTime.HasValue)
                    {
                        continue;
                    }

                    string? roman = await RomanConverter.Convert(line.LrcText, settings.Mode);

                    if (settings.LineType == RomanizationLineType.Replace)
                    {
                        line.LrcText = roman;
                    }
                    else if (settings.LineType == RomanizationLineType.Append)
                    {
                        line.LrcText = $"{line.LrcText}  {roman}";
                    }
                }
            }
            else if (Mode == LyricEditMode.TextBox)
            {
                IEnumerable<LrcLine> lrcLines = LrcHelper.ParseText(LyricText ?? string.Empty);

                foreach (LrcLine line in lrcLines)
                {
                    if (!line.LrcTime.HasValue)
                    {
                        continue;
                    }

                    string? roman = await RomanConverter.Convert(line.LrcText, settings.Mode);

                    if (settings.LineType == RomanizationLineType.Replace)
                    {
                        line.LrcText = roman;
                    }
                    else if (settings.LineType == RomanizationLineType.Append)
                    {
                        line.LrcText = $"{line.LrcText}  {roman}";
                    }
                }

                LyricText = string.Join(Environment.NewLine, lrcLines.Select(l => l.ToString()));
            }
        }
        catch (Exception e)
        {
            Log.Information(e.ToString());
        }
    }

    [RelayCommand]
    public async Task FixKanji()
    {
        if (Mode == LyricEditMode.ListView)
        {
            foreach (ObservableLrcLine line in LrcLines)
            {
                if (!line.LrcTime.HasValue)
                {
                    continue;
                }

                line.LrcText = await KanjiConverter.Convert(line.LrcText);
            }
        }
        else if (Mode == LyricEditMode.TextBox)
        {
            IEnumerable<LrcLine> lrcLines = LrcHelper.ParseText(LyricText ?? string.Empty);

            foreach (LrcLine line in lrcLines)
            {
                if (!line.LrcTime.HasValue)
                {
                    continue;
                }

                line.LrcText = await KanjiConverter.Convert(line.LrcText);
            }

            LyricText = string.Join(Environment.NewLine, lrcLines.Select(l => l.ToString()));
        }
    }

    [RelayCommand]
    public void CreateShare()
    {
        ShareDialog dialog = App.GetService<ShareDialog>();

        if (Mode == LyricEditMode.ListView)
        {
            dialog.CodeToShare = string.Join(Environment.NewLine, LrcLines.Select(l => l.ToString()));
        }
        else if (Mode == LyricEditMode.TextBox)
        {
            dialog.CodeToShare = LyricText ?? string.Empty;
        }
        dialog.Show();
    }

    [RelayCommand]
    public async Task OpenShare()
    {
        string? text = await App.GetService<IClipboardService>().GetTextAsync();

        if (text?.Contains("sharelrc") ?? false)
        {
            string url = CodeCopy.ParsePublicUrl(text);
            string code = await CodeCopy.GetCode(url);

            if (Mode == LyricEditMode.ListView)
            {
                lrcManager.LoadText(code);
                LrcLines.Reset(lrcManager.LrcList.Select(v => MapperProvider.Map<LrcLine, ObservableLrcLine>(v)));
            }
            else if (Mode == LyricEditMode.TextBox)
            {
                LyricText = code;
            }
        }
        else
        {
            await Task.Delay(1); // Waiting for context menu closed
            Toast.Error("剪贴板未包含分享链接");
        }
    }
}

[ObservableObject]
public partial class ObservableLrcLine : LrcLine
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LrcTimeText))]
    [NotifyPropertyChangedFor(nameof(PreviewText))]
    private TimeSpan? lrcTime = default;

    partial void OnLrcTimeChanged(TimeSpan? value)
    {
        base.LrcTime = value;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LrcTimeText))]
    [NotifyPropertyChangedFor(nameof(PreviewText))]
    private string lrcText = default;

    partial void OnLrcTextChanged(string value)
    {
        base.LrcText = value;
    }

    [ObservableProperty]
    [property: NotMapped]
    private bool isHightlight = false;
}

public enum LyricEditMode
{
    ListView,
    TextBox,
}
