using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoviesService.Models
{
    public class MovieReview
    {
        public string Reviewer { get; set; }
        public string Review { get; set; }
        public string Summary { get; set; }
        public string Url { get; set; }
    }
}
