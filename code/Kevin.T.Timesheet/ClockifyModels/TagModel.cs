namespace Kevin.T.Clockify.Data.Models
{
    public class TagModel
    {
        public string id { get; set; }
        public string name { get; set; }

        public string workspaceId { get; set; }
        public bool archived { get; set; }
    }
}
