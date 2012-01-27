using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieInfo.Models
{
    public class BingTranslationResult
    {
        public string TranslatedTerm;
    }

    public class BingTranslation
    {
        public BingTranslationResult[] Results;
    }

    public class BingQuery
    {
        public string SearchTerms;
    }

    public class BingSearchResponse
    {
        public string Version;
        public BingQuery Query;
        public BingTranslation Translation;
    }

    public class BingObj
    {
        public BingSearchResponse SearchResponse;
    }
}