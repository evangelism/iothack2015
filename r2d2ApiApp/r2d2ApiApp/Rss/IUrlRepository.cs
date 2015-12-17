using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace r2d2ApiApp.Rss
{
    public class Url
    {
        public int UrlId { get; set; }
        public string Address { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string ImageUrl { get; set; }
        public string ObjectType { get; set; }
    }
    public interface IUrlRepository
    {
        IQueryable<Url> GetAll();
        Url Get(int id);
        Url Add(Url url);
    }
}
