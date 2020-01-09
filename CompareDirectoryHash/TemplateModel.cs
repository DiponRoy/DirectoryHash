using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareDirectoryHash
{
    public class TemplateModel
    {
        public bool? IgnoreVersion { get; set; }
        public string ConsiderOnlyExtensions { get; set; }
        public bool? IgnoreHashForFailedDisassemble { get; set; }
        public string DisassembleExtensions { get; set; }
        public string IgnorExtensions { get; set; }
        public List<HashCompareModel> Candidates { get; set; }
    }
}
