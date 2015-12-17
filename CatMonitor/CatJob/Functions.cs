using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace CatJob
{
    public class KittieInfo : TableEntity
    {
        public KittieInfo(string Image,string Category)
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

    public class Functions
    {

        private static bool IsCatPresent(AnalysisResult res)
        {
            foreach (var c in res.Categories)
            {
                if (c.Name.ToLower().Contains("cat"))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsPersonPresent(AnalysisResult res)
        {
            foreach (var c in res.Categories)
            {
                if (c.Name.ToLower().Contains("people"))
                {
                    return true;
                }
            }
            return false;
        }

        public static VisionServiceClient OxfordClient = new VisionServiceClient("ce3d37851dd447698bd867471bd8c3c3");

        public static string MajorCategory(AnalysisResult res)
        {
            if (IsCatPresent(res)) return "CAT";
            if (IsPersonPresent(res)) return "HUMAN";
            return "NONE";
        }

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public async static Task ProcessBlob([BlobTrigger("kitties/{name}")] Stream input, string name)
        {
            // output = new Dictionary<Tuple<string, string>, KittieInfo>();
            Console.WriteLine(">> Processing {0}", name);
            var ms = new MemoryStream();
            await input.CopyToAsync(ms);
            ms.Position = 0;
            var res = await (OxfordClient.AnalyzeImageAsync(ms));
            if (res == null) Console.WriteLine("NULL");
            else
            {
                var s = MajorCategory(res);
                Console.WriteLine("{0} ===> {1}", name, s);
                await StoreData(new KittieInfo(name, s));
            }
        }

        private static async Task StoreData(KittieInfo ki)
        {
            CloudStorageAccount sa = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=r2d2storage;AccountKey=UV+6L2Scr9nJAyaLp+jjZflRyr6K05guMafOFJQcZ85NMUOcA4oQmRFzmMR3djiV3gaYkr2z2rmC9Uol9dNPfg==;BlobEndpoint=https://r2d2storage.blob.core.windows.net/;TableEndpoint=https://r2d2storage.table.core.windows.net/;QueueEndpoint=https://r2d2storage.queue.core.windows.net/;FileEndpoint=https://r2d2storage.file.core.windows.net/");
            var ts = sa.CreateCloudTableClient();
            var tab = ts.GetTableReference("KittyData");
            await tab.CreateIfNotExistsAsync();
            var top = TableOperation.Insert(ki);
            await tab.ExecuteAsync(top);
        }
    }
}
