using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ServiceModel.Syndication;

namespace r2d2ApiApp.Rss
{
    public class SyndicationFeedFormatter : MediaTypeFormatter
    {
        private readonly string atom = "application/atom+xml";
        private readonly string rss = "application/rss+xml";

        //public SyndicationFeedFormatter()
        //{
        //    SupportedMediaTypes.Add(new MediaTypeHeaderValue(atom));
        //    SupportedMediaTypes.Add(new MediaTypeHeaderValue(rss));
        //}
        public SyndicationFeedFormatter(string format)
        {
            this.AddUriPathExtensionMapping("rss", new MediaTypeHeaderValue(format));
            this.AddQueryStringMapping("formatter", "rss", new MediaTypeHeaderValue(format));
        }

        Func<Type, bool> SupportedType = (type) =>
        {
            if (type == typeof(Url) || type == typeof(IEnumerable<Url>))
                return true;
            else
                return false;
        };

        public override bool CanReadType(Type type)
        {
            return SupportedType(type);
        }

        public override bool CanWriteType(Type type)
        {
            return SupportedType(type);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, System.Net.TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                if (type == typeof(Url) || type == typeof(IEnumerable<Url>))
                    BuildSyndicationFeed(value, writeStream, content.Headers.ContentType.MediaType);
            });
        }

        private void BuildSyndicationFeed(object models, Stream stream, string contenttype)
        {
            List<SyndicationItem> items = new List<SyndicationItem>();
            var feed = new SyndicationFeed()
            {
                Title = new TextSyndicationContent("My Feed")
            };

            if (models is IEnumerable<Url>)
            {
                var enumerator = ((IEnumerable<Url>)models).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    items.Add(BuildSyndicationItem(enumerator.Current));
                }
            }
            else
            {
                items.Add(BuildSyndicationItem((Url)models));
            }

            feed.Items = items;

            using (
                
                XmlWriter writer = XmlWriter.Create(stream))
            {
                if (string.Equals(contenttype, atom))
                {
                    Atom10FeedFormatter atomformatter = new Atom10FeedFormatter(feed);
                    atomformatter.WriteTo(writer);
                }
                else
                {
                    Rss20FeedFormatter rssformatter = new Rss20FeedFormatter(feed);
                    rssformatter.WriteTo(writer);
                }
            }
        }

        private SyndicationItem BuildSyndicationItem(Url u)
        {
            var item = new SyndicationItem()
            {
                Title = new TextSyndicationContent(u.Title),
                Summary = new TextSyndicationContent(u.Description), // + " <img src=\"" + "http://www.mrwallpaper.com/wallpapers/cute-kitty-playing.jpg" + "\"/>"),
                BaseUri = new Uri(u.Address),
                LastUpdatedTime = u.CreatedAt,                
                Content = new TextSyndicationContent(u.Description) //new CDataSyndicationContent(u.Description + " <img src=\"" + "http://www.mrwallpaper.com/wallpapers/cute-kitty-playing.jpg" + "\"/>")
            };

            item.Authors.Add(new SyndicationPerson() { Name = u.CreatedBy });
            return item;
        }
    }
}
