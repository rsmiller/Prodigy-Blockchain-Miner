
namespace ProdigyBlockchain.BusinessLayer.Dto
{
    public class MinerRequestBlockResponseDto
    {
        public BlockDto block { get; set; }
        public Guid node_id { get; set; }
        public bool has_block { get { return this.block != null; } }
        public DateTimeOffset response_time { get; set; }
    }
}
