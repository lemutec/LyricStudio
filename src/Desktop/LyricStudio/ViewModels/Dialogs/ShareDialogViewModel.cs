using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fischless.Design.Controls;
using LyricStudio.Core.ShareCode;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LyricStudio.ViewModels;

public partial class ShareDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _qrCodeString;

    [ObservableProperty]
    private double _qrCodeSize = 250;

    [ObservableProperty]
    private Thickness _qrCodePadding = new(10);

    [ObservableProperty]
    private CornerRadius _qrCodeCornerRadius = new(12);

    [ObservableProperty]
    private Color _qrCodeForegroundColor1;

    [ObservableProperty]
    private Color _qrCodeForegroundColor2;

    [ObservableProperty]
    private Color _qrCodeBackgroundColor1;

    [ObservableProperty]
    private Color _qrCodeBackgroundColor2;

    [ObservableProperty]
    private QrCode.EccLevel _qrCodeEccLevel;

    private const string Chars = "qwertyuiopasdfghjklzxcvbnm";

    [ObservableProperty]
    private int expirationDateIndex = 1;

    public string CodeToShare { get; set; } = string.Empty;

    public ShareDialogViewModel()
    {
        ResetQrCode();
    }

    [RelayCommand]
    private void UpdateQrCode(string text)
    {
        if (string.IsNullOrEmpty(text)) text = "You didn't put anything here?";
        QrCodeString = text;
    }

    [RelayCommand]
    private void RandomizeData()
    {
        UpdateQrCode(string.Join(string.Empty, Enumerable.Range(0, 150).Select(_ => Chars[Random.Shared.Next(0, Chars.Length)])));
    }

    [RelayCommand]
    private void RandomizeColors()
    {
        var newColors = new byte[12];
        Random.Shared.NextBytes(newColors);

        QrCodeForegroundColor1 = Color.FromRgb(newColors[0], newColors[1], newColors[2]);
        QrCodeForegroundColor2 = Color.FromRgb(newColors[3], newColors[4], newColors[5]);

        QrCodeBackgroundColor1 = Color.FromRgb(newColors[6], newColors[7], newColors[8]);
        QrCodeBackgroundColor2 = Color.FromRgb(newColors[9], newColors[10], newColors[11]);
    }

    [RelayCommand]
    private void ResetQrCode()
    {
        QrCodeEccLevel = QrCode.EccLevel.Medium;

        QrCodeString = "请点击创建分享";

        QrCodeForegroundColor1 = Colors.Navy;
        QrCodeForegroundColor2 = Colors.DarkRed;
        QrCodeBackgroundColor1 = Colors.White;
        QrCodeBackgroundColor2 = Colors.White;
    }

    [RelayCommand]
    private async Task CreateShareQrCode()
    {
        string url = await CodeCopy.PostCode(CodeToShare, shareSecret: "sharelrc", expireTime: ExpirationDateIndex switch
        {
            0 => DateTime.Now.AddDays(1),
            1 => DateTime.Now.AddDays(3),
            2 => DateTime.Now.AddDays(7),
            3 or _ => null!
        });

        if (!string.IsNullOrEmpty(url))
        {
            QrCodeString = url;

            // Once the QR code is generated, could not be generated again.
            await Task.Delay(int.MaxValue);
        }
    }
}
