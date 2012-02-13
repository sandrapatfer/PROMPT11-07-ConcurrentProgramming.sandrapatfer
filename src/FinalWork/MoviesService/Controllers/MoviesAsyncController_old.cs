using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using MoviesService.Models;
using System.Web.Script.Serialization;
using System.IO;
using System.Threading.Tasks;

namespace MoviesService.Controllers
{
    public class MoviesAsyncControllerXXX : AsyncController
    {
        private string _flickrKey = "36cc70e9ab052bac584eb556fbd171ae";
        private string _nyTimesKey = "e13917dc15002cbb0c90046b8c2edb42:5:65635886";
        private string _bingKey = "0C358B2E021F09EBDC6E69DCBB24FC5532A3EC8B";

        //
        // GET: /MoviesAsync/
        [HttpGet]
        public void IndexAsync(string t, string y, string l)
        {
            AsyncManager.OutstandingOperations.Increment();
            if (string.IsNullOrEmpty(t))
            {
                // error
                AsyncManager.Parameters["result"] = null;
                AsyncManager.OutstandingOperations.Decrement(); 
                return;
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

            var info = new MovieInfo();
            var detailsTask = imdbTask.ContinueWith(task => ProcessIMDbResult(task, info, l));
            detailsTask.ContinueWith(task =>
                {
                    AsyncManager.Parameters["result"] = info;
                    AsyncManager.OutstandingOperations.Decrement();
                });
        }

        public ActionResult IndexCompleted(MovieInfo result)
        {
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void ProcessIMDbResult(Task<HttpResponseMessage> task, MovieInfo info, string l)
        {
            IMDbObj imdbObj = null;
            if (task.Result.IsSuccessStatusCode)
            {
                // TODO error handling

                JavaScriptSerializer jsonMaster = new JavaScriptSerializer();
                // TODO?
                var task2 = task.Result.Content.ReadAsStreamAsync();
                imdbObj = jsonMaster.Deserialize<IMDbObj>(new StreamReader(task2.Result).ReadToEnd());
            }

            info.Title = imdbObj.Title;
            info.Year = imdbObj.Year;
            info.Director = imdbObj.Director;
            info.CoverUrl = imdbObj.Poster;

            var httpClient = new HttpClient();
            var flickrUrl = string.Format("http://api.flickr.com/services/rest/?method=flickr.photos.search&api_key={0}&format=json&nojsoncallback=1&text={1}+{2}&sort=interestingness-desc",
                _flickrKey, imdbObj.Title, imdbObj.Director);
            var taskFlickr = httpClient.GetAsync(flickrUrl);
            taskFlickr.ContinueWith(ProcessFlickrResult, TaskContinuationOptions.AttachedToParent);

            var year = Convert.ToInt32(imdbObj.Year);
            var nyTimesUrl = string.Format("http://api.nytimes.com/svc/movies/v2/reviews/search.json?query={0}&api-key={1}&opening-date={2}-01-01;{3}-12-31",
                imdbObj.Title, _nyTimesKey, year, year + 1);
            var taskNYTimes = httpClient.GetAsync(nyTimesUrl);
            taskNYTimes.ContinueWith(ProcessNYTimesResult, TaskContinuationOptions.AttachedToParent);

            if (!string.IsNullOrEmpty(l))
            {
                var bingUrl = string.Format("http://api.bing.net/json.aspx?AppId={0}&Query={PLOT|CAPSULE-REVIEW}&Sources=Translation&Version=2.2&Translation.SourceLanguage=en&Translation.TargetLanguage={1}",
                    _bingKey, l);
                var taskBing = httpClient.GetAsync(bingUrl);
                taskBing.ContinueWith(ProcessBingResult, TaskContinuationOptions.AttachedToParent);
            }
        }

        public void ProcessFlickrResult (Task<HttpResponseMessage> task)
        {
            if (task.Result.IsSuccessStatusCode)
            {
                JavaScriptSerializer jsonMaster = new JavaScriptSerializer();
                var task4 = task.Result.Content.ReadAsStreamAsync();
                var t1 = jsonMaster.Deserialize<FlickrObj>(new StreamReader(task4.Result).ReadToEnd());
                List<PhotoObj> photoList = null;
                photoList = t1.Photos.Photo;
            }
        }

        public void ProcessNYTimesResult(Task<HttpResponseMessage> task)
        {
        }

        public void ProcessBingResult(Task<HttpResponseMessage> task)
        {
        }
    }
}
