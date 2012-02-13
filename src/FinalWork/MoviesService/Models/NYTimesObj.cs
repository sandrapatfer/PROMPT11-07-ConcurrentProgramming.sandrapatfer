using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoviesService.Models
{
    public class NYTimesObj
    {
        public int num_results { get; set; }
        public List<NYTimesCriticObj> results { get; set; }
    }

    public class NYTimesCriticObj
    {
        public string byline { get; set; }
        public string summary_short { get; set; }
        public string capsule_review { get; set; }
        public CriticLinkObj link { get; set; }
    }

    public class CriticLinkObj
    {
        public string url { get; set; }
    }
}