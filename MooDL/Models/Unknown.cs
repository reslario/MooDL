using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MooDL.Models
{
    class Unknown : Resource
    {
        public Unknown(string url, string name) : base(url, name)
        {
        }

        public override string Extension { get; } = ".txt";
        public override string Type { get; } = "Unknown";
    }
}
