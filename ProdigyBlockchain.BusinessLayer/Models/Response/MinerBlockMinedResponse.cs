﻿
namespace ProdigyBlockchain.BusinessLayer.Models.Dto
{
    public class MinerBlockMinedResponse
    {
        public bool validated { get; set; } = false;
        public decimal reward { get; set; }
        public string hash { get; set; }
    }
}
