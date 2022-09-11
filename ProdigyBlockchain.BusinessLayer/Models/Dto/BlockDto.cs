using ProdigyBlockchain.BusinessLayer.Blockchain;

namespace ProdigyBlockchain.BusinessLayer.Dto
{
    public class BlockDto
    {
		public Guid BlockId { get; set; }
		public string OrderNumber { get; set; }
		public string SerialNumber { get; set; }
		public string PreviousHash { get; set; }
		public int Nonce { get; set; }
		public long CreatedOn { get; set; }
		public long MinedOn { get; set; }
		public string Data { get; set; }
		public string Hash { get; set; }
		public List<Transaction> TransactionList { get; set; }
	}
}
