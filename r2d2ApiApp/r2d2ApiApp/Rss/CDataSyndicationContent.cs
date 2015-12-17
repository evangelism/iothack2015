using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace r2d2ApiApp.Rss
{
    public class CDataSyndicationContent : SyndicationContent
    {
        public CDataSyndicationContent(string content)
        {
            Text = content;
        }

        public override SyndicationContent Clone()
        {
            return new CDataSyndicationContent(Text);
        }

        public override string Type
        {
            get { return "html"; }
        }

        public string Text { get; private set; }

        protected override void WriteContentsTo(XmlWriter writer)
        {
            writer.WriteCData(Text);
        }
    }
}
