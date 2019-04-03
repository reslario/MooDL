namespace MooDL.Models.Resources
{
    internal class Docx : Resource
    {
        public Docx(string url, string name) : base(url, name.Replace(".docx", "").Replace(".doc", ""))
        {
        }

        public override string Extension => ".docx";

        public override string Type => "Word";
    }
}