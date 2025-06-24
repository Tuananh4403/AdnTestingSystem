using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Repositories.Models
{
    public class Blog
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime PublishedAt { get; set; }

        public int AuthorId { get; set; }
        public User Author { get; set; } = null!;
    }

}
