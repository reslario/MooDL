namespace MooDL.Models.Resources
{
    class Zip : Resource
    {
        public Zip(string url, string name) : base(url,name.Replace(".zip", ""))
        {
        }

        public override string Extension { get; } = ".zip";

        public override string Type { get; } = "Data";
    }
}
