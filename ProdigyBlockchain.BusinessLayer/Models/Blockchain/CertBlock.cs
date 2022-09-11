using Newtonsoft.Json;

namespace ProdigyBlockchain.BusinessLayer.Blockchain
{
	public interface ICertBlock
	{
		Guid BlockId { get; set; }
		string OrderNumber { get; set; }
		string SerialNumber { get; set; }
		long CreatedOn { get; set; }
		long MinedOn { get; set; }
		int Index { get; set; }
		string PreviousHash { get; set; }
		string Hash { get; set; }
		int Nonce { get; set; }
		byte[] Data { get; set; }
		List<Transaction> TransactionList { get; set; }
	}

	public class CertBlock : ICertBlock
	{
		public Guid BlockId { get; set; } = Guid.NewGuid();
		public string OrderNumber { get; set; }
		public string SerialNumber { get; set; }
		public string PreviousHash { get; set; }
		public string Hash { get; set; }
		public int Nonce { get; set; }
		public int Index { get; set; }
		public long CreatedOn { get; set; }
		public long MinedOn { get; set; }
		public byte[] Data { get; set; }
		public List<Transaction> TransactionList { get; set; } = new List<Transaction>();

		public string GenerateHash()
		{
			var merkleRootHash = FindMerkleRootHash(this.TransactionList);
			return CryptoService.CalculateHash($"{this.Data} + {this.PreviousHash} + {this.CreatedOn} + {JsonConvert.SerializeObject(this.Data) + this.Nonce + merkleRootHash}");
		}

		private string FindMerkleRootHash(IList<Transaction> transactionList)
		{
			var transactionStrList = transactionList.Select(tran => CryptoService.CalculateHash(CryptoService.CalculateHash(tran.from + tran.to + tran.amount))).ToList();
			return BuildMerkleRootHash(transactionStrList);
		}

		private string BuildMerkleRootHash(IList<string> merkelLeaves)
		{
			if (merkelLeaves == null || !merkelLeaves.Any())
				return string.Empty;

			if (merkelLeaves.Count() == 1)
				return merkelLeaves.First();

			if (merkelLeaves.Count() % 2 > 0)
				merkelLeaves.Add(merkelLeaves.Last());

			var merkleBranches = new List<string>();

			for (int i = 0; i < merkelLeaves.Count(); i += 2)
			{
				var leafPair = string.Concat(merkelLeaves[i], merkelLeaves[i + 1]);
				merkleBranches.Add(CryptoService.CalculateHash(CryptoService.CalculateHash(leafPair)));
			}
			return BuildMerkleRootHash(merkleBranches);
		}

		public virtual CertBlock Mine(int difficulty)
		{

			if (this.Data == null)
				throw new ArgumentException(nameof(this.Data));

			string hash_to_match = "";
			this.Nonce = 0; // Setting nonce for proof of work

            while (this.Hash != hash_to_match)
            {
				hash_to_match = this.GenerateHash();
				this.Nonce++;
			}

            this.MinedOn = DateTime.Now.ToFileTimeUtc();

			return this;
		}
	}
}
