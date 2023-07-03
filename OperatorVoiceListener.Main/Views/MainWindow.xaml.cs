using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.UI.Composition.SystemBackdrops;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OperatorVoiceListener.Main.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            
            Closed += MainWindow_Closed;
            Title = "Operator Voice Listener";
            if (MicaController.IsSupported())
            {
                SystemBackdrop = new MicaBackdrop();
            }
            else
            {
                SystemBackdrop = new DesktopAcrylicBackdrop();
            }

            SetTitleBar(AppTitleBar);
            _ = MainFrame.Navigate(typeof(MainPage));
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            Environment.Exit(0);
        }
    }
}
