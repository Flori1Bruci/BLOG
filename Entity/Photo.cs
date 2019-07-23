using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Entity
{
    public class Photo
    {
        public Photo()
        {
        }

        public int Id { get; set; }
        public int ArticleId { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }

        public virtual Article Article { get; set; }
    }
}
