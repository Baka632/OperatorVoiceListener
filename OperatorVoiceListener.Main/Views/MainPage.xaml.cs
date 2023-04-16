using OperatorVoiceListener.Main.Models;
using OperatorVoiceListener.Main.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OperatorVoiceListener.Main.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; }

        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = new MainViewModel();
            MediaPlayerElement.SetMediaPlayer(ViewModel.AudioService.Player);
        }

        private async void StartVoicePlay(object sender, RoutedEventArgs e)
        {
            await ViewModel.StartVoicePlay();
        }

        private void OnSearchCodenameAutoSuggestBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = ViewModel.FindOperatorCodename(sender.Text);
            }
        }

        private void OnVoiceIdAutoSuggestBoxGotFocus(object sender, RoutedEventArgs e)
        {
            ((AutoSuggestBox)sender).ItemsSource = ViewModel.CurrentOperatorVoiceIds;
        }
    }
}
