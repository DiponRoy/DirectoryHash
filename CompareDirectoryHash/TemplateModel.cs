using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareDirectoryHash
{
    public class TemplateModel
    {
        public string IgnorExtensions { get; set; }
        public List<HashCompareModel> Candidates { get; set; }
    }
}
