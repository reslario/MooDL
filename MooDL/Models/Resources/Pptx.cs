namespace MooDL.Models.Resources
{
    internal class Pptx : Resource
    {
        public Pptx(string url, string name) : base(url, name.Replace(".pptx", "").Replace(".ppt", ""))
        {
        }

        public override string Extension => ".pptx";

        public override string Type => "PowerPoint";
    }
}