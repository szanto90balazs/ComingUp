using ComingUp.ViewModels;
using ReactiveUI;
using System.Reactive.Linq;
using Windows.UI.Xaml.Controls;
using AutoSuggestBoxSuggestionEventHandler = Windows.Foundation.TypedEventHandler<Windows.UI.Xaml.Controls.AutoSuggestBox, Windows.UI.Xaml.Controls.AutoSuggestBoxSuggestionChosenEventArgs>;

namespace ComingUp
{
	public sealed partial class MainPage : IViewFor<AppViewModel>
	{
		public MainPage()
		{
			ViewModel = new AppViewModel();
			InitializeComponent();

			Observable.FromEventPattern<AutoSuggestBoxSuggestionEventHandler, AutoSuggestBoxSuggestionChosenEventArgs>(
					x => MediaSuggestionsBox.SuggestionChosen += x,
					x => MediaSuggestionsBox.SuggestionChosen -= x).
				Select(eventPattern => eventPattern.EventArgs.SelectedItem as MediaViewModel).
				InvokeCommand(ViewModel.SuggestionChosen);
		}

		object IViewFor.ViewModel
		{
			get { return ViewModel; }
			set { ViewModel = (AppViewModel)value; }
		}

		public AppViewModel ViewModel { get; set; }
	}
}