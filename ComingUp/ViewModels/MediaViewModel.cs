using ReactiveUI;
using System.Net.TMDb;
using System.Reactive.Linq;

namespace ComingUp.ViewModels
{
	public class MediaViewModel : ReactiveObject
	{
		private Show show;

		public Show Show
		{
			get { return show; }
			set { this.RaiseAndSetIfChanged(ref show, value); }
		}

		private readonly ObservableAsPropertyHelper<string> backdrop;
		public string Backdrop => backdrop.Value;

		private readonly ObservableAsPropertyHelper<string> name;

		public string Name => name.Value;

		public MediaViewModel()
		{
			backdrop = this.WhenAnyValue(p => p.Show).
				Where(show => show != null).
				Select(show => $"http://image.tmdb.org/t/p/original{show.Backdrop?.Trim()}").
				ToProperty(this, p => p.Backdrop);

			name = this.WhenAnyValue(p => p.Show).
				Where(show => show != null).
				Select(show => show.Name?.Trim()).
				ToProperty(this, p => p.Name);
		}
	}
}