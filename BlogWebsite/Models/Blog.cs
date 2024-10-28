using System;
using System.Collections.Generic;

namespace BlogWebsite.Models
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string PostContent { get; set; }
        public string ImageUrl { get; set; }
        public DateTime DatePosted { get; set; }

        // Navigation property for category
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        // Navigation property for comments
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
