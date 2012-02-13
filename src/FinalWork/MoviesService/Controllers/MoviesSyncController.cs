using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using MoviesService.Models;
using System.Web.Script.Serialization;
using System.IO;
using System.Json;

namespace MoviesService.Controllers
{
    public class MoviesSyncController : Controller
    {
        //
        // GET: /MoviesSync/

        public ActionResult Index(string t, string y, string l)
        {
            if (string.IsNullOrEmpty(t))
                // error
                return Json(null, JsonRequestBehavior.AllowGet);

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
            var task = httpClient.GetAsync(imdbUrl);
            IMDbObj imdbObj = null;
            if (task.Result.IsSuccessStatusCode)
            {
                JavaScriptSerializer jsonMaster = new JavaScriptSerializer();
                var task2 = task.Result.Content.ReadAsStreamAsync();
                imdbObj = jsonMaster.Deserialize<IMDbObj>(new StreamReader(task2.Result).ReadToEnd());
            }
            // TODO handle errors 

            string flickrKey = "36cc70e9ab052bac584eb556fbd171ae";
            string flickrUrl = string.Format("http://api.flickr.com/services/rest/?method=flickr.photos.search&api_key={0}&format=json&nojsoncallback=1&text={1}+{2}&sort=interestingness-desc",
                flickrKey, imdbObj.Title, imdbObj.Director);

            var httpClient2 = new HttpClient();
            var task3 = httpClient.GetAsync(flickrUrl);
            List<PhotoObj> photoList = null;
            if (task3.Result.IsSuccessStatusCode)
            {
/*                var task4 = task.Result.Content.ReadAsAsync<JsonValue>();
                var list = task4.Result;*/

                JavaScriptSerializer jsonMaster = new JavaScriptSerializer();
                var task4 = task3.Result.Content.ReadAsStreamAsync();
                var t1 = jsonMaster.Deserialize<FlickrObj>(new StreamReader(task4.Result).ReadToEnd());
                photoList = t1.Photos.Photo;
            }
            // TODO handle errors 

            string farmUrl0 = string.Format("http://farm{0}.static.flickr.com/{1}/{2}_{3}.jpg",
                photoList[0].Farm, photoList[0].Server, photoList[0].Id, photoList[0].Secret);

            return Json(null, JsonRequestBehavior.AllowGet);
        }

    }
}
