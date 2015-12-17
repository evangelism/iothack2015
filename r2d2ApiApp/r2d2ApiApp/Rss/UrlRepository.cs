using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Table;

namespace r2d2ApiApp.Rss
{
    public class UrlRepository : IUrlRepository
    {
        private List<Url> _urls = new List<Url>();
        private int _nextId = 1;

        public UrlRepository()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                "DefaultEndpointsProtocol=https;AccountName=r2d2storage;AccountKey=UV+6L2Scr9nJAyaLp+jjZflRyr6K05guMafOFJQcZ85NMUOcA4oQmRFzmMR3djiV3gaYkr2z2rmC9Uol9dNPfg==;BlobEndpoint=https://r2d2storage.blob.core.windows.net/;TableEndpoint=https://r2d2storage.table.core.windows.net/;QueueEndpoint=https://r2d2storage.queue.core.windows.net/;FileEndpoint=https://r2d2storage.file.core.windows.net/");

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("KittyData");

            TableQuery<KittieInfo> query = new TableQuery<KittieInfo>();

            int i = 0;
            foreach (KittieInfo entity in table.ExecuteQuery(query))
            {
                i++;
                this.Add(new Url()
                {
                    UrlId = i,
                    Address = "https://r2d2storage.blob.core.windows.net/kitties/" + entity.Image,
                    Title = entity.Category + " is here!",
                    CreatedBy = entity.Category,
                    CreatedAt = entity.Moment,
                    Description = "Description" + i.ToString()
                });
            }

            //CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            //CloudBlobContainer container = blobClient.GetContainerReference("kitties");

            //int i = 0;
            //foreach (IListBlobItem item in container.ListBlobs(null, false))
            //{
            //    i++;
            //    if (item.GetType() == typeof(CloudBlockBlob))
            //    {
            //        CloudBlockBlob blob = (CloudBlockBlob)item;

            //        this.Add(new Url()
            //        {
            //            UrlId = i,
            //            Address = blob.Uri.ToString(),
            //            Title = "Test" + i.ToString(),
            //            CreatedBy = "Author" + i.ToString(),
            //            CreatedAt = blob.Properties.LastModified.Value.LocalDateTime,
            //            Description = "Description" + i.ToString()
            //        });

            //    }
            //}

            //this.Add(new Url()
            //{
            //    UrlId = 1,
            //    Address = "http://www.strathweb.com/2012/03/build-facebook-style-infinite-scroll-with-knockout-js-and-last-fm-api/",
            //    Title = "Build Facebook style infinite scroll with knockout.js and Last.fm API",
            //    CreatedBy = "Filip",
            //    CreatedAt = new DateTime(2012, 3, 20),
            //    Description = "Since knockout.js is one of the most amazing and innovative pieces of front-end code I have seen in recent years, I hope this is going to help you a bit in your everday battles. In conjuction with Last.FM API, we are going to create an infinitely scrollable history of your music records – just like the infinite scroll used on Facebook or on Twitter."
            //});
            //this.Add(new Url()
            //{
            //    UrlId = 2,
            //    Address = "http://www.strathweb.com/2012/04/your-own-sports-news-site-with-espn-api-and-knockout-js/",
            //    Title = "Your own sports news site with ESPN API and Knockout.js",
            //    CreatedBy = "Filip",
            //    CreatedAt = new DateTime(2012, 4, 8),
            //    Description = "You will be able to browse the latest news from ESPN from all sports categories, as well as filter them by tags. The UI will be powered by KnockoutJS and Twitter bootstrap, and yes, will be a single page. We have already done two projects together using knockout.js – last.fm API infinite scroll and ASP.NET WebAPI file upload. Hopefully we will continue our knockout.js adventures in an exciting, and interesting for you, way."
            //});
            //this.Add(new Url()
            //{
            //    UrlId = 3,
            //    Address = "http://www.strathweb.com/2012/04/your-own-sports-news-site-with-espn-api-and-knockout-js/",
            //    Title = "Test ПРИВЕТ!",
            //    CreatedBy = "XaocCPS",
            //    CreatedAt = new DateTime(2015, 12, 16),
            //    Description = "THIS IS THE WIN!"
            //});
        }

        public IQueryable<Url> GetAll()
        {
            return _urls.AsQueryable();
        }
        public Url Get(int id)
        {
            return _urls.Find(i => i.UrlId == id);
        }
        public Url Add(Url url)
        {
            url.UrlId = _nextId++;
            _urls.Add(url);
            return url;
        }
    }
}
