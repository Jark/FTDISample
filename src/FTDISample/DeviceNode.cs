namespace FTDISample
{
    public class DeviceNode
    {
        public string Description { get; }
        public string Id { get; }

        public DeviceNode(string id, string description)
        {
            Id = id;
            Description = description;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Description, Id);
        }
    }
}