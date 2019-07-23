using System;
using System.Collections.Generic;

namespace Blog.Entity
{
    public partial class Tag
    {
        public Tag()
        {
            ArticleTag = new HashSet<ArticleTag>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ArticleTag> ArticleTag { get; set; }
    }
}
