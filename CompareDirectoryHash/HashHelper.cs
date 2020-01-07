/*https://stackoverflow.com/a/15683147/2948523*/

using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CompareDirectoryHash
{

    class HashHelper
    {
        public const string templatePath = "Template.xml";
        public const string reultPath = "Result.csv";
        public readonly StringBuilder Result;

        public readonly List<HashCompareModel> Candidates;
        public readonly List<string> IgnorExtensions;
        public readonly string ExportLocation;
        
        public HashHelper()
        {
            var serializer = new XmlSerializer(typeof(TemplateModel));
            var reader = new StreamReader(templatePath);
            var model = (TemplateModel)serializer.Deserialize(reader);
            reader.Close();
            Candidates = model.Candidates;
            IgnorExtensions = model.IgnorExtensions.Split(',').Select(x => x.Trim()).ToList();

            Result = new StringBuilder();
        }


        public string DorectoryMd5Hash(string path)
        {
            // assuming you want to include nested folders
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                 .Where(file => !IgnorExtensions.Any(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
                                 .OrderBy(p => p)
                                 .ToList();

            MD5 md5 = MD5.Create();

            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];

                // hash path
                string relativePath = file.Substring(path.Length + 1);
                byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLower());
                md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                // hash contents
                byte[] contentBytes = File.ReadAllBytes(file);
                if (i == files.Count - 1)
                {
                    md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                }
                else
                {
                    md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
                }
            }

            return BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
        }


        public void AddResultLine(string line)
        {
            Result.AppendLine(line);
        }

        public void CreateResultFile()
        {
            if (File.Exists(reultPath))
            {
                File.Delete(reultPath);
            }
            File.WriteAllText(reultPath, Result.ToString());
        }
    }
}
