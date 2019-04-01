using System.Text.RegularExpressions;

namespace MooDL.Models.Resources
{
    abstract class Resource
    {
        private const string SupportedTypes = "unknown|document|spreadsheet|powerpoint|archive|pdf|icon";

        public const string FolderResourceRegex = @"class=""fp-filename-icon""><a href=""([^""]+)""><[^>]*><(?:\w* \w*=""[^""]*""){3}\s*src=""https:\/\/moodle\.bbbaden\.ch\/(?:[^\/]+\/)*(" + SupportedTypes + @")[^>]*><\/span><span[^>]*>([^<]+)";

        public const string ResourceRegex = @"<div class=""activityinstance""><a class="""" onclick="""" href=""([^""]+)""><img src=""https:\/\/moodle\.bbbaden\.ch\/(?:[^\/]+\/)*(?:folder|core)\/\d*\/(?:f\/)?(" + SupportedTypes + @")[^<]*[^>]*>([^<]*)";

        public abstract string Extension { get; }

        public abstract string Type { get; }

        public string Url { get; }

        public string Name { get; }

        protected Resource(string url, string name)
        {
            Url = url;
            Name = Regex.Replace(name, @"(?:\/|\\|\?|%|\*|:|\||""|<|>|\.)", " ");
        }
    }
}
