using System.Security.Cryptography;
using System.Text;

namespace ProdigyBlockchain.BusinessLayer
{
	public class CryptoService
	{
		public static byte[] GenerateHash(byte[] hashingData)
		{
			using (SHA256 sha256 = SHA256.Create())
			{
                return SHA512.Create().ComputeHash(sha256.ComputeHash(hashingData));
            }
		}

		public static string CalculateHash(string stringForHashing)
		{
			string hash = String.Empty;
			byte[] crypto = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(stringForHashing), 0, Encoding.ASCII.GetByteCount(stringForHashing));

			foreach (byte theByte in crypto)
			{
				hash += theByte.ToString("x2");
			}

			return hash;
		}
	}
}
