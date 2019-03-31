using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MooDL.Models.Resources
{
    class Folder : Resource
    {
        public override string Extension => "";

        public override string Type => "Folders";

        public Resource[] Resources { get; set; }

        public Folder(string url, string name) : base(url, name)
        {
        }
    }
}
