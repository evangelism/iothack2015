using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace r2d2ApiApp.Rss
{
    public class KittieInfo : TableEntity
    {
        public KittieInfo()
        {

        }
        public KittieInfo(string Image, string Category)
        {
            this.PartitionKey = "MAIN";
            this.RowKey = Image;
            this.Image = Image;
            this.Category = Category;
            this.Moment = DateTime.Now;
        }

        public string Image { get; set; }
        public DateTime Moment { get; set; }
        public string Category { get; set; }
    }

}
