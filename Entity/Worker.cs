using Blog.Helpers;
using System;
using System.Collections.Generic;

namespace Blog.Entity
{
    public partial class Worker
    {
        public Worker()
        {
            Article = new HashSet<Article>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public DateTime Joined { get; set; }
        public Position Position { get; set; }
        public string UserGuid { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }

        public ICollection<Article> Article { get; set; }
    }
}
