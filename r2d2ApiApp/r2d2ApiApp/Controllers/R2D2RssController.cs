using r2d2ApiApp.Rss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace r2d2ApiApp.Controllers
{
    public class R2D2RssController : ApiController
    {
        private static readonly IUrlRepository _repo = new UrlRepository();                
        public IEnumerable<Url> Get()
        {            
            return _repo.GetAll();
        }
        public Url Get(int id)
        {
            return _repo.Get(id);
        }
        public void Post([FromBody]string value)
        {
        }
        public void Put(int id, [FromBody]string value)
        {
        }
        public void Delete(int id)
        {
        }
    }
}
