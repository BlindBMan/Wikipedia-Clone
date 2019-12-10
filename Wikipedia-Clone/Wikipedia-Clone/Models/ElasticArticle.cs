using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocalProject.Models
{
    // class representation for the ElasticSearch
    public class ElasticArticle
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }
        public string CategoryName { get; set; }
    }
}