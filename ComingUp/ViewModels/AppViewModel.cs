using Akavache;
using ComingUp.Utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ComingUp.ViewModels
{
	public class AppViewModel : ReactiveObject
	{
		private readonly TmdbApi tmdbApi;

		private string searchTerm;

		public string SearchTerm
		{
			get { return searchTerm; }
			set { this.RaiseAndSetIfChanged(ref searchTerm, value); }
		}

		public ReactiveCommand<MediaViewModel> SuggestionChosen { get; set; }
		public ReactiveCommand<IEnumerable<MediaViewModel>> ExecuteSearch { get; protected set; }

		private readonly ObservableAsPropertyHelper<IEnumerable<MediaViewModel>> searchResults;
		public IEnumerable<MediaViewModel> SearchResults => searchResults.Value;

		public IReactiveDerivedList<MediaViewModel> SavedMedia { get; }

		public AppViewModel()
		{
			tmdbApi = new TmdbApi();

			ExecuteSearch = ReactiveCommand.CreateAsyncTask(async param => await tmdbApi.GetMovies(SearchTerm));
			this.WhenAnyValue(x => x.SearchTerm).
				Throttle(TimeSpan.FromMilliseconds(800), RxApp.MainThreadScheduler).
				Select(x => x?.Trim()).
				DistinctUntilChanged().
				Where(x => !string.IsNullOrWhiteSpace(x)).
				InvokeCommand(ExecuteSearch);

			searchResults = ExecuteSearch.ToProperty(this, x => x.SearchResults, new List<MediaViewModel>());

			SuggestionChosen = ReactiveCommand.CreateAsyncTask(x => Task.FromResult(x as MediaViewModel));
			SavedMedia = SuggestionChosen.CreateCollection();
			SavedMedia.ItemsAdded.Subscribe(async newlySavedMedia =>
			{
				await BlobCache.LocalMachine.InsertObject(newlySavedMedia.Show.Id.ToString(), newlySavedMedia);
			});

			var allKeys = BlobCache.LocalMachine.GetAllKeys().Wait();
		}
	}
}