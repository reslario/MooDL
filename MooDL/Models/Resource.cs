using System.Text.RegularExpressions;

namespace MooDL.Models
{
    abstract class Resource
    {
        public abstract string Extension { get; }

        public abstract string Type { get; }

        public string Url { get; }

        public string Name { get; }

        protected Resource(string url, string name)
        {
            Url = url;
            Name = new Regex(@"(?:\/|\\|\?|%|\*|:|\||""|<|>|\.)").Replace(name, " ");
        }
    }
}
