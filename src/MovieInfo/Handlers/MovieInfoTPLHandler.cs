using System;
using System.Web;
using System.Net;
using System.Web.Script.Serialization;
using System.IO;
using System.Threading;

namespace MovieInfo
{
    using Models;
    using System.Threading.Tasks;

    public class MovieInfoTPLHandler : IHttpAsyncHandler
    {
        private const int MAX_RETRIES = 8;
        private const string BING_KEY = "0C358B2E021F09EBDC6E69DCBB24FC5532A3EC8B";

        private static readonly Random _rand = new Random();

        public bool IsReusable
        {
            get { return false; }
        }

        private void ReplyError(HttpStatusCode statusCode, string text, HttpResponse response)
        {
            response.StatusCode = (int)statusCode;
            response.ContentType = "text/html";
            response.Write(String.Format("<html><title>MovieInfo : ERROR</title><body><p>{0}</p></body></html>", text));
            for (int i = 0; i < 85; ++i) response.Write("&nbsp;");
        }

        public class AsyncResult<T> : IAsyncResult
        {
            #region IAsyncResult Members

            public object AsyncState
            {
                get;
                set;
            }

            public System.Threading.WaitHandle AsyncWaitHandle
            {
                get;
                set;
            }

            public bool CompletedSynchronously
            {
                get { return false; }
            }

            public bool IsCompleted
            {
                get;
                set;
            }
            public AsyncCallback Callback { get; set; }

            #endregion

            public void OperationComplete()
            {
                IsCompleted = true;
                if (AsyncWaitHandle != null)
                {
                    var ev = AsyncWaitHandle as ManualResetEvent;
                    ev.Set();
                }
                if (Callback != null)
                {
                    Callback(this);
                }
            }
        }

        private HttpContext _context;

        #region IHttpAsyncHandler Members

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            _context = context;

            string movie;
            if (context.Request.QueryString["t"] == null)
            {
                movie = "Casablanca";
            }
            else
            {
                movie = context.Request.QueryString["t"];
            }

            string imdbRequestUri = null;
            if (context.Request.QueryString["y"] == null)
            {
                imdbRequestUri = String.Format("http://imdbapi.com/?t={0}", movie);
            }
            else
            {
                imdbRequestUri = String.Format("http://imdbapi.com/?t={0}&y={1}", movie, context.Request.QueryString["y"]);
            }
            HttpWebRequest imdbRequest = (HttpWebRequest)WebRequest.Create(imdbRequestUri);
            //return _imdbRequest.BeginGetResponse(cb, extraData);

            var result = new AsyncResult<HttpWebResponse>()
            {
                AsyncState = extraData,
                IsCompleted = false,
                AsyncWaitHandle = new ManualResetEvent(false),
                Callback = cb
            };

            var t1 = Task.Factory.FromAsync(imdbRequest.BeginGetResponse, (Func<IAsyncResult,WebResponse>)imdbRequest.EndGetResponse, null);

            imdbRequest.BeginGetResponse((ar) =>
            {
            HttpWebResponse imdbResponse = (HttpWebResponse)imdbRequest.EndGetResponse(ar);
            if (imdbResponse.StatusCode == HttpStatusCode.OK)
            {
                JavaScriptSerializer jsonMaster = new JavaScriptSerializer();
                IMDbObj imdbObj = jsonMaster.Deserialize<IMDbObj>(new StreamReader(imdbResponse.GetResponseStream()).ReadToEnd());
                if (imdbObj != null && imdbObj.Response == "True")
                {
                    if (imdbObj.Plot != "N/A")
                    {
                        string bingRequestUri = String.Format("http://api.bing.net/json.aspx?AppId={0}&Query={1}&Sources=Translation&Version=2.2&Translation.SourceLanguage={2}&Translation.TargetLanguage={3}", BING_KEY, imdbObj.Plot, "en", "pt");
                        /*
                                                for (int retries = 0; retries < MAX_RETRIES; ++retries)
                                                {*/

                        HttpWebRequest bingRequest = (HttpWebRequest)WebRequest.Create(bingRequestUri);
                        bingRequest.BeginGetResponse((ar2) =>
                    {
                        HttpWebResponse bingResponse = (HttpWebResponse)bingRequest.EndGetResponse(ar2);
                        if (bingResponse.StatusCode == HttpStatusCode.OK)
                        {
                            BingObj bingObj = jsonMaster.Deserialize<BingObj>(new StreamReader(bingResponse.GetResponseStream()).ReadToEnd());
                            if (bingObj.SearchResponse.Translation != null)
                            {
                                imdbObj.Plot = bingObj.SearchResponse.Translation.Results[0].TranslatedTerm;
                                //break;
                            }
                        }
                        _context.Response.StatusCode = (int)HttpStatusCode.OK;
                        _context.Response.ContentType = "text/plain; charset=utf-8"; // In the final version use "application/json; charset=utf-8"
                        string jsonData = jsonMaster.Serialize(imdbObj);
                        var writer = new StreamWriter(_context.Response.OutputStream);
                        writer.Write(jsonData);
                        writer.Flush();

                        result.OperationComplete();

                    }, null);
                    }
                }
            }
            },
            null);

            return result;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
                    return;
            //ReplyError(HttpStatusCode.NotFound, "Movie not found", _context.Response);
        }

        #endregion

        #region IHttpHandler Members


        public void ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
