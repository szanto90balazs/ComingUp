using ComingUp.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Net.TMDb;
using System.Threading;
using System.Threading.Tasks;

namespace ComingUp.Utils
{
	public class TmdbApi
	{
		public async Task<IEnumerable<MediaViewModel>> GetMovies(string currentSearchTerm)
		{
			var cancellationToken = new CancellationToken();
			using (var tmdbClient = new ServiceClient("40f2479d476c7f5ae3758bd88b296a7f"))
			{
				var shows = await tmdbClient.Shows.SearchAsync(currentSearchTerm, "en", null, 1, cancellationToken);
				return shows.Results.Select(p => new MediaViewModel
				{
					Show = p
				});
			}
		}
	}
}