namespace MooDL.Models.Resources
{
    class Unknown : Resource
    {
        public Unknown(string url, string name) : base(url, name)
        {
        }

        public override string Extension { get; } = ".txt";
        public override string Type { get; } = "Unknown";
    }
}
