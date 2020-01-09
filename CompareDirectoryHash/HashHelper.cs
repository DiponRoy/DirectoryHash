/*https://stackoverflow.com/a/15683147/2948523*/

using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Reflection;
using static CompareDirectoryHash.Disassembler;

namespace CompareDirectoryHash
{

    class HashHelper
    {
        public const string TemplatePath = "Template.xml";
        public const string ReultPath = "Result.csv";
        public const string IlFailPath = "ILFail.csv";

        public readonly StringBuilder Result;
        public readonly StringBuilder FailReport;

        public readonly bool IgnoreVersion;
        public readonly bool IgnoreHashForFailedDisassemble;
        public readonly List<HashCompareModel> Candidates;
        public readonly List<string> IgnorExtensions;
        public readonly List<string> ConsiderOnlyExtensions;
        public readonly List<string> DisassembleExtensions;
        
        public HashHelper()
        {
            var serializer = new XmlSerializer(typeof(TemplateModel));
            var reader = new StreamReader(TemplatePath);
            var model = (TemplateModel)serializer.Deserialize(reader);
            reader.Close();

            IgnoreVersion = (bool)model.IgnoreVersion;
            IgnoreHashForFailedDisassemble = (bool)model.IgnoreHashForFailedDisassemble;
            Candidates = model.Candidates;
            DisassembleExtensions = SplitToLower(model.DisassembleExtensions);
            ConsiderOnlyExtensions = SplitToLower(model.ConsiderOnlyExtensions);
            IgnorExtensions = SplitToLower(model.IgnorExtensions);
            Result = new StringBuilder();
            FailReport = new StringBuilder();
        }

        private List<string> SplitToLower(string value)
        {
            List<string> values = value.Split(',')
                .Select(x => x.Trim().ToLower())
                .Where(x => !String.IsNullOrEmpty(x))
                .ToList();
            return values;
        }


        //public bool DorectoryHash(string directoryPath, out string hashResult)
        //{
        //    hashResult = string.Empty;
        //    if (!Directory.Exists(directoryPath))
        //    {
        //        hashResult = "Directory not found";
        //        return false;
        //    }

        //    // assuming you want to include nested folders
        //    var files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories)
        //                         .WhereIf(IgnorExtensions.Count > 0, file => !IgnorExtensions.Any(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
        //                         .OrderBy(p => p)
        //                         .ToList();
        //    if (files.Count == 0)
        //    {
        //        hashResult = "Directory is empty";
        //        return false;
        //    }

        //    MD5 md5 = MD5.Create();
        //    for (int i = 0; i < files.Count; i++)
        //    {
        //        string file = files[i];

        //        // hash path
        //        string relativePath = file.Substring(directoryPath.Length + 1);
        //        byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLower());
        //        md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

        //        // hash contents
        //        byte[] contentBytes = File.ReadAllBytes(file);
        //        //byte[] contentBytes = DisassembleExtensions.Count > 0 && DisassembleExtensions.Any(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase)) 
        //        //    ? GetDisassembledBytes(file)
        //        //    : File.ReadAllBytes(file);

        //        if (i == files.Count - 1)
        //        {
        //            md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
        //        }
        //        else
        //        {
        //            md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
        //        }
        //    }

        //    hashResult = BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
        //    return true;
        //}

        public bool DorectoryHash(string directoryPath, out string hashResult)
        {
            hashResult = string.Empty;
            if (!Directory.Exists(directoryPath))
            {
                hashResult = "Directory not found";
                return false;
            }

            // assuming you want to include nested folders
            List<FileModel> files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories)
                                 .Select(x => new FileModel
                                 {
                                     Path = x,
                                     Name = Path.GetFileName(x),
                                     Extension = Path.GetExtension(x).ToLowerInvariant()
                                 })
                                 .WhereIf(ConsiderOnlyExtensions.Count > 0, file => ConsiderOnlyExtensions.Contains(file.Extension))
                                 .WhereIf(IgnorExtensions.Count > 0, file => !IgnorExtensions.Contains(file.Extension))
                                 .OrderBy(x => x.Path)
                                 .ToList();
            if (files.Count == 0)
            {
                hashResult = "Directory is empty";
                return false;
            }

            HashAlgorithm hashService = MD5.Create();
            string hashString = BitConverter.ToString(Hash(hashService, files));
            hashResult = hashString.Replace("-", "").ToLower();
            return true;
        }

        private byte[] Hash(HashAlgorithm hashService, List<FileModel> files)
        {
            byte[] hash = null;
            using (hashService)
            {
                foreach (var file in files)
                {
                    string path = file.Path;
                    string extension = file.Extension;
                    if (DisassembleExtensions.Count > 0 && DisassembleExtensions.Contains(extension))
                    {
                        DissasembleOutput disassembled;
                        if (Disassembler.Disassemble(path, out disassembled))
                        {
                            AddFileToHash(disassembled.ILFilename, hashService, AssemblySourceCleanup.GetFilter(AssemblySourceCleanup.FileTypes.IL, IgnoreVersion));
                            foreach (var resource in disassembled.Resources)
                            {
                                AddFileToHash(resource, hashService,
                                AssemblySourceCleanup.GetFilter(resource, IgnoreVersion));
                            }
                            disassembled.Delete();
                        }
                        else
                        {
                            AddFailLine(file);
                            if (!IgnoreHashForFailedDisassemble)
                            {
                                AddFileToHash(path, hashService, AssemblySourceCleanup.GetFilter(path, IgnoreVersion));
                            }
                        }
                    }
                    else
                    {
                        AddFileToHash(path, hashService, AssemblySourceCleanup.GetFilter(path, IgnoreVersion));
                    }
                }
                hashService.TransformFinalBlock(new byte[0], 0, 0);
                hash = hashService.Hash;
            }
            return hash;
        }

        private void AddFileToHash(string filename, HashAlgorithm hashService, StreamFilter filter = null, Encoding encoding = null)
        {
            if (filter == null || filter == StreamFilter.None)
            {
                using (var stream = File.OpenRead(filename))
                {
                    var buffer = new byte[1200000];
                    var bytesRead = stream.Read(buffer, 0, buffer.Length);
                    while (bytesRead > 1)
                    {
                        hashService.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                    }
                }
            }
            else
            {
                if (encoding == null)
                {
                    if (Path.GetExtension(filename).Equals(".res", StringComparison.InvariantCultureIgnoreCase))
                    {
                        encoding = Encoding.Unicode;
                    }
                    else
                    {
                        encoding = Encoding.Default;
                    }
                }
                using (var stream = File.OpenRead(filename))
                {
                    using (var reader = new StreamReader(stream, encoding))
                    {
                        foreach (var line in filter.ReadAllLines(reader))
                        {
                            var lineBuffer = encoding.GetBytes(line);
                            hashService.TransformBlock(lineBuffer, 0, lineBuffer.Length, lineBuffer, 0);
                        }
                    }
                }
            }
        }

        public void AddResultLine(string line)
        {
            Result.AppendLine(line);
        }

        private void AddFailLine(FileModel file)
        {
            string line = string.Format("{0},{1},{2}", file.Name, file.Extension, file.Path);
            FailReport.AppendLine(line);
        }

        public void CreateResults()
        {
            if (File.Exists(ReultPath))
            {
                File.Delete(ReultPath);
            }
            File.WriteAllText(ReultPath, Result.ToString());

            if (File.Exists(IlFailPath))
            {
                File.Delete(IlFailPath);
            }
            File.WriteAllText(IlFailPath, FailReport.ToString());       
        }
    }
}
