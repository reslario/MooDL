using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MooDL.Models
{
    class Pdf : Resource
    {
        public Pdf(string url, string name) : base(url, name.Replace(".pdf", ""))
        {
        }

        public override string Extension { get; } = ".pdf";
        public override string Type { get; } = "PDF";
    }
}
