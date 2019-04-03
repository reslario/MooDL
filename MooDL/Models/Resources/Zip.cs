namespace MooDL.Models.Resources
{
    internal class Zip : Resource
    {
        public Zip(string url, string name) : base(url, name.Replace(".zip", ""))
        {
        }

        public override string Extension => ".zip";

        public override string Type => "Data";
    }
}