using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoviesService.Models
{
    public class FlickrPhotosObj
    {
        public int Total { get; set; }
        public List<PhotoObj> Photo { get; set; }
    }
}
