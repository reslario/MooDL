using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MooDL.Models
{
    class Zip : Resource
    {
        public Zip(string url, string name) : base(url,name.Replace(".zip", ""))
        {
        }

        public override string Extension { get; } = ".zip";

        public override string Type { get; } = "Data";
    }
}
