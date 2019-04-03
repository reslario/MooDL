namespace MooDL.Models.Resources
{
    internal class Folder : Resource
    {
        public Folder(string url, string name) : base(url, name)
        {
        }

        public override string Extension => "";

        public override string Type => "Folders";
    }
}