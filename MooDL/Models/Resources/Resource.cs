﻿using System.Text.RegularExpressions;

namespace MooDL.Models
{
    abstract class Resource
    {
        public const string FOLDER_RESOURCE_REGEX = @"class=""fp-filename-icon""><a href=""([^""]+)""><[^>]*><(?:\w* \w*=""[^""]*""){3}\s*src=""https:\/\/moodle\.bbbaden\.ch\/theme\/image\.php\/lambda\/core\/1547110846\/f\/(unknown|document|spreadsheet|powerpoint|archive|pdf)[^>]*><\/span><span[^>]*>([^<]+)";

        public const string RESOURCE_REGEX = @"<div class=""activityinstance""><a class="""" onclick="""" href=""([^""]+)""><img src=""https:\/\/moodle\.bbbaden\.ch\/(?:[^\/]+\/)*(unknown|document|spreadsheet|powerpoint|archive|pdf|icon)[^<]*[^>]*>([^<]*)";

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