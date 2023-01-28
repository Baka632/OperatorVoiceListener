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
            ViewModel = new MainViewModel(DispatcherQueue);
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

        private void OnSearchCodenameAutoSuggestBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion is not null)
            {
                OperatorCodenameInfo codenameInfo = (OperatorCodenameInfo)args.ChosenSuggestion;
                ViewModel.OperatorCodename = sender.Text = codenameInfo.Codename;
            }
            else
            {
                ViewModel.OperatorCodename = args.QueryText;
            }
        }

        private void OnSearchCodenameAutoSuggestBoxLostFocus(object sender, RoutedEventArgs e)
        {
            AutoSuggestBox autoSuggestBox = (AutoSuggestBox)sender;
            ViewModel.OperatorCodename = autoSuggestBox.Text;
        }

        private void OnSearchVoiceIdAutoSuggestBoxGotFocus(object sender, RoutedEventArgs e)
        {
            AutoSuggestBox autoSuggestBox = (AutoSuggestBox)sender;
            autoSuggestBox.ItemsSource = ViewModel.FindCurrentOperatorVoiceId();
        }

        private void OnSearchVoiceIDAutoSuggestBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion is not null)
            {
                OperatorIdTitleInfo codenameInfo = (OperatorIdTitleInfo)args.ChosenSuggestion;
                ViewModel.VoiceID = sender.Text = codenameInfo.Id;
            }
            else
            {
                ViewModel.VoiceID = args.QueryText;
            }
        }

        private void OnSearchVoiceIDAutoSuggestBoxLostFocus(object sender, RoutedEventArgs e)
        {
            AutoSuggestBox autoSuggestBox = (AutoSuggestBox)sender;
            ViewModel.VoiceID = autoSuggestBox.Text;
        }
    }
}
