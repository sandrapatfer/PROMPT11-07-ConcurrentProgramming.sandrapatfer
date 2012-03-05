using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MoviesService.Models;
using System.Threading.Tasks;

namespace MoviesService.Utils
{
    public class CacheController
    {
        private static CacheController _singleton;
        public static CacheController Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    _singleton = new CacheController();
                }
                return _singleton;
            }
        }

        private AsyncCache<string, AsyncCache<string, MovieInfo>> _movieCache;

        public CacheController()
        {
            _movieCache = new AsyncCache<string, AsyncCache<string, MovieInfo>>((movieKey) =>
            {
                var movieProcTask = RequestProcessor.CreateMovieRequestIterator(movieKey).Run<MovieInfo>();
                return movieProcTask.ContinueWith((taskMovie) =>
                {
                    return new AsyncCache<string, MovieInfo>((languageKey) =>
                    {
                        return RequestProcessor.CreateTranslatorRequestIterator(languageKey,
                            new MovieInfo(taskMovie.Result)).Run<MovieInfo>();
                    });
                });
            });
        }

        public Task<MovieInfo> GetRequest(string t, string y, string l)
        {
            if (string.IsNullOrEmpty(t))
            {
                var tcs1 = new TaskCompletionSource<MovieInfo>();
                tcs1.SetResult(null);
                return tcs1.Task;
            }

            string imdbUrl;
            if (string.IsNullOrEmpty(y))
            {
                imdbUrl = string.Format("http://imdbapi.com/?t={0}&plot=full", t);
            }
            else
            {
                imdbUrl = string.Format("http://imdbapi.com/?t={0}&y={1}&plot=full", t, y);
            }

            var tcs2 = new TaskCompletionSource<MovieInfo>();
            _movieCache.Get(imdbUrl).ContinueWith(taskMovieCache =>
            {
                if (string.IsNullOrEmpty(l))
                {
                    l = "en";
                }
                taskMovieCache.Result.Get(l).ContinueWith(taskMovie =>
                {
                    tcs2.SetResult(taskMovie.Result);
                });
            });
            return tcs2.Task;
        }
    }
}