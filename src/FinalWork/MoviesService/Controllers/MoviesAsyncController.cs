using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.IO;
using System.Threading.Tasks;
using MoviesService.Models;
using MoviesService.Utils;

namespace MoviesService.Controllers
{
    public class MoviesAsyncController : AsyncController
    {
        private const int _MAX_RETRIES = 8;
        private static readonly Random __rand = new Random();

        private const string _flickrKey = "36cc70e9ab052bac584eb556fbd171ae";
        private const string _nyTimesKey = "e13917dc15002cbb0c90046b8c2edb42:5:65635886";
        private const string _bingKey = "0C358B2E021F09EBDC6E69DCBB24FC5532A3EC8B";

        //
        // GET: /MoviesAsync/
        [HttpGet]
        public void IndexAsync(string t, string y, string l)
        {
            AsyncManager.OutstandingOperations.Increment();

            CreateMainProcessRequestIterator(t, y, l).Run<MovieInfo>().ContinueWith(task =>
            {
                AsyncManager.Parameters["result"] = task.Result;
                AsyncManager.OutstandingOperations.Decrement();
            });
        }

        public ActionResult IndexCompleted(MovieInfo result)
        {
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public IEnumerator<Task> CreateMainProcessRequestIterator(string t, string y, string l)
        {
            if (string.IsNullOrEmpty(t))
            {
                var tcs1 = new TaskCompletionSource<MovieInfo>();
                tcs1.SetResult(null);
                yield return tcs1.Task;
                yield break;
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
                CoverUrl = imdbObj.Poster
            };

            var taskWhenAll = TaskUtils.WhenAll(
                CreateFlickrTask(imdbObj, info),
                CreateNYTimesTask(imdbObj, info, l),
                CreateBingTask(imdbObj, info, l)
            );
            yield return taskWhenAll;

            var tcs3 = new TaskCompletionSource<MovieInfo>();
            tcs3.SetResult(info);
            yield return tcs3.Task;
        }

        private Task CreateFlickrTask(IMDbObj imdbObj, MovieInfo info)
        {
            var httpClient = new HttpClient();
            var flickrUrl = string.Format("http://api.flickr.com/services/rest/?method=flickr.photos.search&api_key={0}&format=json&nojsoncallback=1&text={1}+{2}&sort=interestingness-desc",
                _flickrKey, imdbObj.Title, imdbObj.Director);
            var taskFlickr = httpClient.GetAsync(flickrUrl);
            return taskFlickr.ContinueWith(t =>
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
                    });
                }
            });
        }
        private Task CreateNYTimesTask(IMDbObj imdbObj, MovieInfo info, string l)
        {
            return ProcessNYTimesIterator(imdbObj, info, l).Run();
        }

        private IEnumerator<Task> ProcessNYTimesIterator(IMDbObj imdbObj, MovieInfo info, string l)
        {
            var year = Convert.ToInt32(imdbObj.Year);
            var nyTimesUrl = string.Format("http://api.nytimes.com/svc/movies/v2/reviews/search.json?query={0}&api-key={1}&opening-date={2}-01-01;{3}-12-31",
                imdbObj.Title, _nyTimesKey, year, year + 1);
            var httpClient = new HttpClient();

            var requestTask = httpClient.GetAsync(nyTimesUrl);
            yield return requestTask;

            if (requestTask.Result.IsSuccessStatusCode)
            {
                var taskReadObject = requestTask.Result.Content.ReadAsStreamAsync();
                yield return taskReadObject;

                JavaScriptSerializer jsonMaster = new JavaScriptSerializer();
                var nyTimesObj = jsonMaster.Deserialize<NYTimesObj>(new StreamReader(taskReadObject.Result).ReadToEnd());
                if (nyTimesObj.num_results > 0)
                {
                    // should the translations be parallel? the bing service wouldn't support it...

                    info.NYTimesReviews = new List<MovieReview>();
                    foreach (var critic in nyTimesObj.results)
                    {
                        string summary = "";
                        if (!string.IsNullOrEmpty(critic.summary_short))
                        {
                            var taskBing = ProcessBingIterator(critic.summary_short, l).Run<string>();
                            yield return taskBing;
                            summary = taskBing.Result;
                        }
                        string review = "";
                        if (!string.IsNullOrEmpty(critic.capsule_review))
                        {
                            var taskBing = ProcessBingIterator(critic.capsule_review, l).Run<string>();
                            yield return taskBing;
                            review = taskBing.Result;
                        }
                        info.NYTimesReviews.Add(new MovieReview()
                        {
                            Reviewer = critic.byline,
                            Summary = summary,
                            Review = review,
                            Url = critic.link.url
                        });
                    }
                }
            }
        }

        private Task CreateBingTask(IMDbObj imdbObj, MovieInfo info, string l)
        {
            if (!string.IsNullOrEmpty(l))
            {
                return ProcessBingIterator(imdbObj.Plot, l).Run<string>().ContinueWith(task =>
                {
                    info.Description = task.Result;
                });
            }
            else
            {
                info.Description = imdbObj.Plot;
                var tcs = new TaskCompletionSource<MovieInfo>();
                tcs.SetResult(null);
                return tcs.Task;
            }
        }

        private IEnumerator<Task> ProcessBingIterator(string textToTranslate, string l)
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
                    if (bingObj.SearchResponse.Translation != null)
                    {
                        var tcs = new TaskCompletionSource<string>();
                        tcs.SetResult(bingObj.SearchResponse.Translation.Results[0].TranslatedTerm);
                        yield return tcs.Task;
                        yield break;
                    }
                }

                var taskDelay = TaskUtils.NewDelayTask(1000 + 1000 * retries + __rand.Next(2000));
                yield return taskDelay;
            }

            // the process was not successfull
            var tcs2 = new TaskCompletionSource<string>();
            tcs2.SetResult(textToTranslate);
            yield return tcs2.Task;
        }
    }
}
