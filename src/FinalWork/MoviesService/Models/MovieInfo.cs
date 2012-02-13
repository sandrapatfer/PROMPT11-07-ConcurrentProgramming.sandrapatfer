using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoviesService.Models
{
    public class MovieInfo
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string Director { get; set; }
        public string Description { get; set; }
        public string CoverUrl { get; set; }
        public List<string> FlickrPhotos { get; set; }
        public List<MovieReview> NYTimesReviews { get; set; }
    }
}