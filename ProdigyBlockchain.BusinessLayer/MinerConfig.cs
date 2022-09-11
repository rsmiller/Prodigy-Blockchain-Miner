
namespace ProdigyBlockchain.BusinessLayer
{
    public interface IMinerConfig
    {
        string url { get; set; }
        int port { get; set; }
        string wallet_address { get; set; }
    }

    public class MinerConfig: IMinerConfig
    {
        public string url { get; set; }
        public int port { get; set; }
        public string wallet_address { get; set; }
    }
}
