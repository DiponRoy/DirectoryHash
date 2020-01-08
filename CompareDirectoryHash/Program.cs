using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CompareDirectoryHash
{
    class Program
    {
        const string nullValue = "NONE";

        static void Main(string[] args)
        {
            //CreateTemplate();
            CompareHash();
            Console.ReadKey();
        }

        private static void CompareHash()
        {
            try
            {
                var helper = new HashHelper();
                helper.AddResultLine(string.Format("{0},{1},{2},{3},{4},{5}", "Result", "FirstHash", "SecondHash", "Name", "FirstDorectory", "SecondDorectory"));

                bool hasFirstHash, hasSecondHash;
                string firstHash, secondHash, result;
                foreach (var model in helper.Candidates)
                {
                    hasFirstHash = hasSecondHash = false;
                    firstHash = secondHash = result = string.Empty;
                    hasFirstHash = helper.DorectoryMd5Hash(model.FirstDorectory, out firstHash);
                    hasSecondHash = helper.DorectoryMd5Hash(model.SecondDorectory,out secondHash);
                    result = !hasFirstHash || !hasSecondHash ? nullValue : firstHash.Equals(secondHash).ToString();

                    string line = string.Format("{0},{1},{2},{3}", result, firstHash, secondHash, model.Name);
                    helper.AddResultLine(string.Format("{0},{1},{2}", line, model.FirstDorectory, model.SecondDorectory));
                    Console.WriteLine(line);
                }
                helper.CreateResultFile();
            }
            catch (Exception error)
            {
                Console.WriteLine("Error");
                Console.Write(error.Message);
            }
            Console.WriteLine();
            Console.WriteLine("Finish");
        }

        private static void CreateTemplate()
        {
            var model = new TemplateModel();
            model.IgnorExtensions = @".json, .exe";
            model.Candidates = new List<HashCompareModel>()
            {
                new HashCompareModel()
                {
                    Name = "Name",
                    FirstDorectory =  @"C:\",
                    SecondDorectory =  @"C:\",
                }
            };
            XmlSerializer x = new XmlSerializer(model.GetType());
            x.Serialize(Console.Out, model);
        }
    }
}
