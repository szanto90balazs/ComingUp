using Windows.UI.Xaml.Controls;
using ComingUp.ViewModels;

namespace ComingUp
{
    public sealed partial class MainPage : Page
    {
		public AppViewModel AppViewModel { get; }

        public MainPage()
        {
	        AppViewModel = new AppViewModel();
            InitializeComponent();
        }
	}
}
