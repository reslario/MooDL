namespace MooDL.Models.Resources
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
