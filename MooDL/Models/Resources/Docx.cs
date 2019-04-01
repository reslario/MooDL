namespace MooDL.Models.Resources
{
    class Docx : Resource
    {
        public override string Extension { get; } = ".docx";

        public override string Type { get; } = "Word";

        public Docx(string url, string name) : base(url, name.Replace(".docx", "").Replace(".doc", ""))
        {

        }
    }
}
