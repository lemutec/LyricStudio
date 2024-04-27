using Avalonia.Controls;
using Fischless.Design.Controls;
using Fischless.Globalization;
using LyricStudio.Services;
using LyricStudio.ViewModels;

namespace LyricStudio.Views;

public partial class MainView : UserControl
{
    public MainViewViewModel ViewModel { get; }

    public MainView() : this(App.GetService<MainViewViewModel>())
    {
    }

    public MainView(MainViewViewModel viewModel)
    {
        //var d = Avalonia.Markup.Xaml.MarkupExtensions.I18NExtension.Translate("HomePage");
        //var d23 = Fischless.Globalization.LangExtension.Translate("HomePage");
        //var d2 = Fischless.Globalization.LangExtension.Translate("MediaInfoDropHint");
        //var dsf = MuiLanguage.Mui("HomePage");
        DataContext = ViewModel = viewModel;
        InitializeComponent();
        ViewLocator.Services = App.Services;
        App.GetService<INavigationService>()?.SetFrame(FrameView);
        App.GetService<INavigationService>()?.SetNavigationView(NavigationView);
        App.GetService<INavigationService>()?.Navigate(typeof(HomePage));
    }
}
