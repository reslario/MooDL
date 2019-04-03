namespace MooDL.Models.Resources
{
    internal class Xlsx : Resource
    {
        public Xlsx(string url, string name) : base(url, name.Replace(".xlsx", ""))
        {
        }

        public override string Extension => ".xlsx";
        public override string Type => "Excel";
    }
}