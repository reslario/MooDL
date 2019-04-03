namespace MooDL.Models.Resources
{
    internal class Pdf : Resource
    {
        public Pdf(string url, string name) : base(url, name.Replace(".pdf", ""))
        {
        }

        public override string Extension => ".pdf";
        public override string Type => "PDF";
    }
}