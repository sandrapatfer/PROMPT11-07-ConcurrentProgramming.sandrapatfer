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
        public string Plot { get; set; }
        public string CoverUrl { get; set; }
        public List<string> FlickrPhotos { get; set; }
        public List<MovieReview> NYTimesReviews { get; set; }

        public MovieInfo() { }

        public MovieInfo(MovieInfo other)
        {
            this.Title = other.Title;
            this.Year = other.Year;
            this.Director = other.Director;
            this.Plot = other.Plot;
            this.CoverUrl = other.CoverUrl;

            // no need to copy photo links
            this.FlickrPhotos = other.FlickrPhotos;

            // copy reviews
            if (other.NYTimesReviews != null)
            {
                this.NYTimesReviews = new List<MovieReview>();
                foreach (var review in other.NYTimesReviews)
                {
                    this.NYTimesReviews.Add(new MovieReview(review));
                }
            }
        }
    }
}