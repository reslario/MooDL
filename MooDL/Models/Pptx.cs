namespace MooDL.Models
{
    class Pptx : Resource
    {
        public Pptx(string url, string name) : base(url, name.Replace(".pptx", "").Replace(".ppt", ""))
        {
        }

        public override string Extension { get; } = ".pptx";

        public override string Type { get; } = "PowerPoint";
    }
}
