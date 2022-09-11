using ProdigyBlockchain.BusinessLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace ProdigyBlockchain.BusinessLayer.Command
{
    public class MinerBlockMinedCommand
    {
        [Required]
        public MiningUser miner { get; set; }
        [Required]
        public Guid block_id { get; set; }
        [Required]
        public string block_hash { get; set; }
    }
}
