using System;
using System.Web;
using System.Net;
using System.Web.Script.Serialization;
using System.IO;
using System.Threading;

namespace MovieInfo
{
    using Models;

    public class MovieInfoHandler : IHttpHandler
    {
        private const int _MAX_RETRIES = 8;
        private const string BING_KEY = "0C358B2E021F09EBDC6E69DCBB24FC5532A3EC8B";

        private static readonly Random __rand = new Random();

        public bool IsReusable
        {
            get { return true; }
        }

        private void ReplyError(HttpStatusCode statusCode, string text, HttpResponse response)
        {
            response.StatusCode = (int)statusCode;
            response.ContentType = "text/html";
            response.Write(String.Format("<html><title>MovieInfo : ERROR</title><body><p>{0}</p></body></html>", text));
            for (int i = 0; i < 85; ++i) response.Write("&nbsp;");
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.Path != "/")
            {
                ReplyError(HttpStatusCode.NotFound, "Resource does not exist", context.Response);
                return;
            }

            if (context.Request.QueryString["t"] == null)
            {
                ReplyError(HttpStatusCode.BadRequest, "Requests must indicate a movie via parameter <b>t=<i>movie</i></b>", context.Response);
                return;
            }

            string imdbRequestUri = null;
            if (context.Request.QueryString["y"] == null) {
                imdbRequestUri = String.Format("http://imdbapi.com/?t={0}", context.Request.QueryString["t"]);
            } else {
                imdbRequestUri = String.Format("http://imdbapi.com/?t={0}&y={1}", context.Request.QueryString["t"], context.Request.QueryString["y"]);
            }

            HttpWebRequest imdbRequest = (HttpWebRequest)WebRequest.Create(imdbRequestUri);
            HttpWebResponse imdbResponse = (HttpWebResponse)imdbRequest.GetResponse();

            if (imdbResponse.StatusCode == HttpStatusCode.OK) {
                JavaScriptSerializer jsonMaster = new JavaScriptSerializer();
                IMDbObj imdbObj = jsonMaster.Deserialize<IMDbObj>(new StreamReader(imdbResponse.GetResponseStream()).ReadToEnd());
                if (imdbObj != null && imdbObj.Response == "True") {
                    if (imdbObj.Plot != "N/A")
                    {
                        string bingRequestUri = String.Format("http://api.bing.net/json.aspx?AppId={0}&Query={1}&Sources=Translation&Version=2.2&Translation.SourceLanguage={2}&Translation.TargetLanguage={3}", BING_KEY, imdbObj.Plot, "en", "pt");

                        for (int retries = 0; retries < _MAX_RETRIES; ++retries) {

                            HttpWebRequest bingRequest = (HttpWebRequest)WebRequest.Create(bingRequestUri);
                            HttpWebResponse bingResponse = (HttpWebResponse)bingRequest.GetResponse();

                            if (bingResponse.StatusCode == HttpStatusCode.OK)
                            {
                                BingObj bingObj = jsonMaster.Deserialize<BingObj>(new StreamReader(bingResponse.GetResponseStream()).ReadToEnd());
                                if (bingObj.SearchResponse.Translation != null)
                                {
                                    imdbObj.Plot = bingObj.SearchResponse.Translation.Results[0].TranslatedTerm;
                                    break;
                                }
                            }

                            Thread.Sleep(1000 + 1000 * retries + __rand.Next(2000));
                        }
                    }
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.ContentType = "text/plain; charset=utf-8"; // In the final version use "application/json; charset=utf-8"
                    string jsonData = jsonMaster.Serialize(imdbObj);
                    var writer = new StreamWriter(context.Response.OutputStream);
                    writer.Write(jsonData);
                    writer.Flush();
                    return;
                }
            }

            ReplyError(HttpStatusCode.NotFound, "Movie not found", context.Response);
        }
    }
}
