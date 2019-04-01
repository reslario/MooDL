namespace MooDL.Models.Resources
{
    class Xlsx : Resource
    {
        public Xlsx(string url, string name) : base(url, name.Replace(".xlsx", ""))
        {
        }

        public override string Extension { get; } = ".xlsx";
        public override string Type { get; } = "Excel";
    }
}
