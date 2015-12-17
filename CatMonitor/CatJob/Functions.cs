using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace CatJob
{
    public class KittieInfo
    {
        public string UrlImage { get; set; }
        public int EventType { get; set; }
        public DateTime Moment { get; set; }
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
            }
        }
    }
}
