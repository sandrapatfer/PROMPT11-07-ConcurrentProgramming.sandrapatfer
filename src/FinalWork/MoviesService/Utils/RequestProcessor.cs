using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MoviesService.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.IO;
using System.Collections;
using Utils;

namespace MoviesService.Utils
{
    public class RequestProcessor
    {
        private const int _MAX_RETRIES = 8;
        private static readonly Random __rand = new Random();

        private const string _flickrKey = "36cc70e9ab052bac584eb556fbd171ae";
        private const string _nyTimesKey = "e13917dc15002cbb0c90046b8c2edb42:5:65635886";
        private const string _bingKey = "0C358B2E021F09EBDC6E69DCBB24FC5532A3EC8B";

        public static IEnumerator<Task> CreateMovieRequestIterator(string imdbUrl)
        {
            var httpClient = new HttpClient();
            var imdbTask = httpClient.GetAsync(imdbUrl);
            yield return imdbTask;

            if (!imdbTask.Result.IsSuccessStatusCode)
            {
                var tcs2 = new TaskCompletionSource<MovieInfo>();
                tcs2.SetResult(null);
                yield return tcs2.Task;
                yield break;
            }

            var taskReadImdbObject = imdbTask.Result.Content.ReadAsStreamAsync();
            yield return taskReadImdbObject;

            JavaScriptSerializer jsonMaster = new JavaScriptSerializer();
            IMDbObj imdbObj = jsonMaster.Deserialize<IMDbObj>(new StreamReader(taskReadImdbObject.Result).ReadToEnd());

            var info = new MovieInfo()
            {
                Title = imdbObj.Title,
                Year = imdbObj.Year,
                Director = imdbObj.Director,
                CoverUrl = imdbObj.Poster,
                Plot = imdbObj.Plot
            };

            var taskWhenAll = TaskUtils.WhenAll(
                CreateFlickrTask(imdbObj, info),
                CreateNYTimesTask(imdbObj, info)
            );
            yield return taskWhenAll;

            var tcs3 = new TaskCompletionSource<MovieInfo>();
            tcs3.SetResult(info);
            yield return tcs3.Task;
        }

        public static IEnumerator<Task> CreateTranslatorRequestIterator(string language, MovieInfo movieInfo)
        {
            if (string.IsNullOrEmpty(language))
            {
                var tcs = new TaskCompletionSource<MovieInfo>();
                tcs.SetResult(movieInfo);
                yield return tcs.Task;
                yield break;
            }
            else
            {
                var tasks = new List<Task>();
                if (!string.IsNullOrEmpty(movieInfo.Plot))
                {
                    tasks.Add(
                        ProcessBingIterator(movieInfo.Plot, language).Run<string>().ContinueWith(task1 =>
                        {
                            movieInfo.Plot = task1.Result;
                        }));
                }
                if (movieInfo.NYTimesReviews != null)
                {
                    foreach (var review in movieInfo.NYTimesReviews)
                    {
                        if (!string.IsNullOrEmpty(review.Summary))
                        {
                            tasks.Add(ProcessBingIterator(review.Summary, language).Run<string>().ContinueWith(task =>
                            {
                                review.Summary = task.Result;
                            }));
                        }

                        if (!string.IsNullOrEmpty(review.Review))
                        {
                            tasks.Add(ProcessBingIterator(review.Review, language).Run<string>().ContinueWith(task =>
                            {
                                review.Review = task.Result;
                            }));
                        }
                    }
                }
                var taskWhenAll = TaskUtils.WhenAll(tasks.ToArray());
                yield return taskWhenAll;

                var tcs3 = new TaskCompletionSource<MovieInfo>();
                tcs3.SetResult(movieInfo);
                yield return tcs3.Task;
            }

        }
        
        private static Task CreateFlickrTask(IMDbObj imdbObj, MovieInfo info)
        {
            var tcs = new TaskCompletionSource<MovieInfo>();
            var httpClient = new HttpClient();
            var flickrUrl = string.Format("http://api.flickr.com/services/rest/?method=flickr.photos.search&api_key={0}&format=json&nojsoncallback=1&text={1}+{2}&sort=interestingness-desc",
                _flickrKey, imdbObj.Title, imdbObj.Director);
            var taskFlickr = httpClient.GetAsync(flickrUrl);
            taskFlickr.ContinueWith(t =>
            {
                if (t.Result.IsSuccessStatusCode)
                {
                    var taskReadFlickrObj = t.Result.Content.ReadAsStreamAsync();
                    taskReadFlickrObj.ContinueWith(t2 =>
                    {
                        JavaScriptSerializer jsonMaster = new JavaScriptSerializer();
                        var flickrInfo = jsonMaster.Deserialize<FlickrObj>(new StreamReader(t2.Result).ReadToEnd());
                        info.FlickrPhotos = flickrInfo.Photos.Photo.Select(p =>
                            string.Format("http://farm{0}.static.flickr.com/{1}/{2}_{3}.jpg",
                            p.Farm, p.Server, p.Id, p.Secret)).ToList();
                        tcs.SetResult(info);
                    });
                }
            });
            return tcs.Task;
        }
        private static Task CreateNYTimesTask(IMDbObj imdbObj, MovieInfo info)
        {
            var tcs = new TaskCompletionSource<MovieInfo>();
            var year = Convert.ToInt32(imdbObj.Year);
            var nyTimesUrl = string.Format("http://api.nytimes.com/svc/movies/v2/reviews/search.json?query={0}&api-key={1}&opening-date={2}-01-01;{3}-12-31",
                imdbObj.Title, _nyTimesKey, year, year + 1);
            var httpClient = new HttpClient();

            var requestTask = httpClient.GetAsync(nyTimesUrl);
            requestTask.ContinueWith(t1 =>
            {
                if (t1.Result.IsSuccessStatusCode)
                {
                    var taskReadObject = requestTask.Result.Content.ReadAsStreamAsync();
                    taskReadObject.ContinueWith(t2 =>
                    {
                        JavaScriptSerializer jsonMaster = new JavaScriptSerializer();
                        var nyTimesObj = jsonMaster.Deserialize<NYTimesObj>(new StreamReader(t2.Result).ReadToEnd());
                        if (nyTimesObj.num_results > 0)
                        {
                            info.NYTimesReviews = new List<MovieReview>();
                            foreach (var critic in nyTimesObj.results)
                            {
                                info.NYTimesReviews.Add(new MovieReview()
                                {
                                    Reviewer = critic.byline,
                                    Summary = critic.summary_short,
                                    Review = critic.capsule_review,
                                    Url = critic.link.url
                                });
                            }
                            tcs.SetResult(info);
                        }
                    });
                }
            });
            return tcs.Task;
        }

        private static IEnumerator<Task> ProcessBingIterator(string textToTranslate, string l)
        {
            if (!string.IsNullOrEmpty(l))
            {
                var bingUrl = string.Format("http://api.bing.net/json.aspx?AppId={0}&Query={1}&Sources=Translation&Version=2.2&Translation.SourceLanguage=en&Translation.TargetLanguage={2}",
                    _bingKey, textToTranslate, l);
                var httpClient = new HttpClient();
                for (int retries = 0; retries < _MAX_RETRIES; ++retries)
                {
                    var requestTask = httpClient.GetAsync(bingUrl);
                    yield return requestTask;

                    if (requestTask.Result.IsSuccessStatusCode)
                    {
                        var taskReadObject = requestTask.Result.Content.ReadAsStreamAsync();
                        yield return taskReadObject;

                        JavaScriptSerializer jsonMaster = new JavaScriptSerializer();
                        var bingObj = jsonMaster.Deserialize<BingObj>(new StreamReader(taskReadObject.Result).ReadToEnd());
                        var tcs = new TaskCompletionSource<string>();
                        if (bingObj.SearchResponse.Translation != null)
                        {
                            tcs.SetResult(bingObj.SearchResponse.Translation.Results[0].TranslatedTerm);
                        }
                        else
                        {
                            // translation failed, but the status code is ok, so no need to repeat the request
                            tcs.SetResult(textToTranslate);
                        }
                        yield return tcs.Task;
                        yield break;
                    }

                    var taskDelay = TaskUtils.NewDelayTask(1000 + 1000 * retries + __rand.Next(2000));
                    yield return taskDelay;
                }
            }

            // return the same text if everything fails
            var tcs2 = new TaskCompletionSource<string>();
            tcs2.SetResult(textToTranslate);
            yield return tcs2.Task;
        }
    }
}