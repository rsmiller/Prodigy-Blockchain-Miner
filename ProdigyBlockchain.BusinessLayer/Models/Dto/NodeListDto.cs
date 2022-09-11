namespace ProdigyBlockchain.BusinessLayer.Dto
{
    public class NodeListDto
    {
        public int difficulty { get; set; }
        public List<string> nodes { get; set; } = new List<string>();
    }
}
