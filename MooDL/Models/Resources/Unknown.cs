namespace MooDL.Models.Resources
{
    internal class Unknown : Resource
    {
        public Unknown(string url, string name) : base(url, name)
        {
        }

        public override string Extension => ".txt";
        public override string Type => "Unknown";
    }
}