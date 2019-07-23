using Blog.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blog.Entity
{
    public partial class Article
    {
        public Article()
        {
            ArticleTag = new HashSet<ArticleTag>();
            Photo = new HashSet<Photo>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Serp { get; set; }
        public string Meta { get; set; }
        public string Body { get; set; }
        public int WorkerId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModifed { get; set; }
        public DateTime? DatePublished { get; set; }
        public Status Status { get; set; }

        public Worker Worker { get; set; }
        public ICollection<ArticleTag> ArticleTag { get; set; }
        public ICollection<Photo> Photo { get; set; }
    }
}
