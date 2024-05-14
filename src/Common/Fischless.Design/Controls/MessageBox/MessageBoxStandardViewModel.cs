using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace Fischless.Design.Controls;

public partial class MessageBoxStandardViewModel : AbstractMessageBoxViewModel
{
    public event Action<MessageBoxResult> CloseRequested;

    public MessageBoxResult Result { get; set; }

    public readonly MessageBoxResult _enterDefaultButton;
    public readonly MessageBoxResult _escDefaultButton;

    public MessageBoxStandardViewModel(MessageBoxStandardParams @params) : base(@params, @params.Icon)
    {
        _enterDefaultButton = @params.EnterDefaultButton;
        _escDefaultButton = @params.EscDefaultButton;

        Text = new MessageBoxButtonText();
        SetButtons(@params.ButtonDefinitions);
        ButtonClickCommand = new RelayCommand<object?>(o => ButtonClick(o.ToString()));
        EnterClickCommand = new RelayCommand(() => EnterClick());
        EscClickCommand = new RelayCommand(() => EscClick());
    }

    public MessageBoxButtonText Text { get; private set; }

    public bool IsOkShowed { get; private set; }
    public bool IsYesShowed { get; private set; }
    public bool IsNoShowed { get; private set; }
    public bool IsAbortShowed { get; private set; }
    public bool IsCancelShowed { get; private set; }

    public override string InputLabel { get; internal set; }
    public override string InputValue { get; set; }
    public override bool IsInputMultiline { get; internal set; }
    public override bool IsInputVisible { get; internal set; }

    public RelayCommand<object?> ButtonClickCommand { get; }
    public RelayCommand EnterClickCommand { get; }
    public RelayCommand EscClickCommand { get; }

    private void SetButtons(MessageBoxButton paramsButtonDefinitions)
    {
        switch (paramsButtonDefinitions)
        {
            case MessageBoxButton.OK:
                IsOkShowed = true;
                break;

            case MessageBoxButton.YesNo:
                IsYesShowed = true;
                IsNoShowed = true;
                break;

            case MessageBoxButton.OKCancel:
                IsOkShowed = true;
                IsCancelShowed = true;
                break;

            case MessageBoxButton.YesNoCancel:
                IsYesShowed = true;
                IsNoShowed = true;
                IsCancelShowed = true;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(paramsButtonDefinitions), paramsButtonDefinitions,
                    null);
        }
    }

    private void EscClick()
    {
        switch (_escDefaultButton)
        {
            case MessageBoxResult.Ok:
                ButtonClick(MessageBoxResult.Ok);
                return;

            case MessageBoxResult.Yes:
                ButtonClick(MessageBoxResult.Yes);
                return;

            case MessageBoxResult.No:
                ButtonClick(MessageBoxResult.No);
                return;

            case MessageBoxResult.Abort:
                ButtonClick(MessageBoxResult.Abort);
                return;

            case MessageBoxResult.Cancel:
                ButtonClick(MessageBoxResult.Cancel);
                return;

            case MessageBoxResult.None:
                ButtonClick(MessageBoxResult.None);
                return;

            //case MessageBoxResult.Default:
            //    {
            //        if (IsCancelShowed)
            //        {
            //            ButtonClick(MessageBoxResult.Cancel);
            //            return;
            //        }

            //        if (IsAbortShowed)
            //        {
            //            ButtonClick(MessageBoxResult.Abort);
            //            return;
            //        }

            //        if (IsNoShowed)
            //        {
            //            ButtonClick(MessageBoxResult.No);
            //            return;
            //        }
            //    }
            //    break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        ButtonClick(MessageBoxResult.None);
    }

    private void EnterClick()
    {
        switch (_enterDefaultButton)
        {
            case MessageBoxResult.Ok:
                ButtonClick(MessageBoxResult.Ok);
                return;

            case MessageBoxResult.Yes:
                ButtonClick(MessageBoxResult.Yes);
                return;

            case MessageBoxResult.No:
                ButtonClick(MessageBoxResult.No);
                return;

            case MessageBoxResult.Abort:
                ButtonClick(MessageBoxResult.Abort);
                return;

            case MessageBoxResult.Cancel:
                ButtonClick(MessageBoxResult.Cancel);
                return;

            case MessageBoxResult.None:
                ButtonClick(MessageBoxResult.None);
                return;

            //case MessageBoxResult.Default:
            //    {
            //        if (IsOkShowed)
            //        {
            //            ButtonClick(MessageBoxResult.Ok);
            //            return;
            //        }

            //        if (IsYesShowed)
            //        {
            //            ButtonClick(MessageBoxResult.Yes);
            //            return;
            //        }
            //    }
            //    break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Close()
    {
        CloseRequested?.Invoke(Result);
    }

    public void SetButtonResult(MessageBoxResult bdName)
    {
        Result = bdName;
    }

    public async void ButtonClick(string parameter)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            SetButtonResult((MessageBoxResult)Enum.Parse(typeof(MessageBoxResult), parameter.Trim(), true));
            Close();
        });
    }

    public async void ButtonClick(MessageBoxResult buttonResult)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            SetButtonResult(buttonResult);
            Close();
        });
    }
}
