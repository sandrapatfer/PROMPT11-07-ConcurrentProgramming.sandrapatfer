using System.Web.Mvc;
using MoviesService.Models;
using MoviesService.Utils;

namespace MoviesService.Controllers
{
    public class MoviesAsyncController : AsyncController
    {
        public MoviesAsyncController()
        {
        }

        //
        // GET: /MoviesAsync/
        [HttpGet]
        public void IndexAsync(string t, string y, string l)
        {
            AsyncManager.OutstandingOperations.Increment();

            RequestProcessor.Singleton.GetRequest(t, y, l).ContinueWith(task =>
            {
                AsyncManager.Parameters["result"] = task.Result;
                AsyncManager.OutstandingOperations.Decrement();
            });
        }

        public ActionResult IndexCompleted(MovieInfo result)
        {
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
