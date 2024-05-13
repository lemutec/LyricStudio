using FluentAvalonia.UI.Controls;

namespace Fischless.Design.Controls;

public class FluentContentDialog : FluentWindow
{
    protected IContentDialogViewModel viewModel = null!;

    public IContentDialogViewModel ViewModel
    {
        get => viewModel;
        protected set
        {
            if (viewModel != null)
            {
                viewModel.CloseRequested -= OnCloseRequested;
            }
            viewModel = value;
            viewModel.CloseRequested += OnCloseRequested;
        }
    }

    protected virtual void OnCloseRequested(ContentDialogResult e)
    {
        Close(e);
    }
}
