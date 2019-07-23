using System;
using System.Collections.Generic;

namespace Blog.Entity
{
    public partial class ArticleTag
    {
        public int TagId { get; set; }
        public int ArticleId { get; set; }

        public Article Article { get; set; }
        public Tag Tag { get; set; }
    }
}
