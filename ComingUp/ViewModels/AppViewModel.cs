using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.TMDb;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ComingUp.ViewModels
{
	public class AppViewModel : ReactiveObject
	{
		private string searchTerm;

		public string SearchTerm
		{
			get { return searchTerm; }
			set
			{
				this.RaiseAndSetIfChanged(ref searchTerm, value);
			}
		}

		public ReactiveCommand<IEnumerable<ShowViewModel>> ExecuteSearch { get; protected set; }

		private ObservableAsPropertyHelper<IEnumerable<ShowViewModel>> searchResults;
		public IEnumerable<ShowViewModel> SearchResults => searchResults.Value;

		public AppViewModel()
		{
			ExecuteSearch = ReactiveCommand.CreateAsyncTask(async param => await GetMovies(SearchTerm));
			this.WhenAnyValue(x => x.SearchTerm).
				Throttle(TimeSpan.FromMilliseconds(800), RxApp.MainThreadScheduler).
				Select(x => x?.Trim()).
				DistinctUntilChanged().
				Where(x => !string.IsNullOrWhiteSpace(x)).
				InvokeCommand(ExecuteSearch);

			searchResults = ExecuteSearch.ToProperty(this, x => x.SearchResults, new List<ShowViewModel>());
		}

		private async Task<IEnumerable<ShowViewModel>> GetMovies(string currentSearchTerm)
		{
			var cancellationToken = new CancellationToken();
			using (var tmdbClient = new ServiceClient("40f2479d476c7f5ae3758bd88b296a7f"))
			{
				var shows = await tmdbClient.Shows.SearchAsync(currentSearchTerm, "en", null, 1, cancellationToken);
				return shows.Results.Select(p => new ShowViewModel
				{
					Show = p
				});
			}
		}
	}
}