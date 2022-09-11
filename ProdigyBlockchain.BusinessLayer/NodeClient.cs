using ProdigyBlockchain.BusinessLayer.Blockchain;
using ProdigyBlockchain.BusinessLayer.Command;
using ProdigyBlockchain.BusinessLayer.Dto;
using ProdigyBlockchain.BusinessLayer.Models;
using ProdigyBlockchain.BusinessLayer.Models.Response;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace ProdigyBlockchain.BusinessLayer
{
    public class NodeClient
    {
        private RestClient _Client;
        private IPAddress _NodeIPAddress;
        private IPAddress _MinerInternalIPAddress;
        private IPAddress _MinerExternalIPAddress;
        private string _WalletId = "";

        public NodeClient(string wallet_id, IPAddress internal_ip_address, IPAddress external_ip_address, IPAddress node_address, int port)
        {
            _WalletId = wallet_id;
            _NodeIPAddress = node_address;
            _MinerInternalIPAddress = internal_ip_address;
            _MinerExternalIPAddress = external_ip_address;

            var url = "http://" + node_address.ToString() + ":" + port + "/api/";

            _Client = new RestClient(url);
        }

        public JoinRequestResponse JoinRequest()
        {
            var request = new RestRequest("Blockchain/MinerJoin?ip_address=" + _NodeIPAddress.ToString(), Method.Get);

            var response = _Client.Execute(request, Method.Get);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<NodeListDto>(response.Content);

                var rand = new Random((int)DateTime.UtcNow.Ticks);

                // Get random node
                return new JoinRequestResponse()
                {
                    difficulty = result.difficulty,
                    ip_address = result.nodes.ElementAt(rand.Next(result.nodes.Count()))
                }; 
            }

            return null;
        }

        public CertBlock RequestBlock()
        {
            var miner = new MiningUser()
            {
                internal_ip_address = _MinerInternalIPAddress.ToString(),
                external_ip_address = _MinerExternalIPAddress.ToString(),
                walled_id = _WalletId
            };

            var request = new RestRequest("Blockchain/RequestBlock", Method.Post);
            request.AddJsonBody(miner);

            var response = _Client.Execute(request, Method.Post);

            if(response.StatusCode == HttpStatusCode.OK)
            {
                var response_data = JsonConvert.DeserializeObject<MinerRequestBlockResponseDto>(response.Content);
                
                if(response_data.has_block)
                {
                    return ConvertDtoToBlock(response_data.block);
                }
            }

            return null;
        }

        public MinerBlockMinedResponse MinedBlock(Guid block_id, string hash)
        {
            var command = new MinerBlockMinedCommand()
            {
                miner = new MiningUser()
                {
                    internal_ip_address = _MinerInternalIPAddress.ToString(),
                    external_ip_address = _MinerExternalIPAddress.ToString(),
                    walled_id = _WalletId
                },
                block_hash = hash,
                block_id = block_id,
            };

            var request = new RestRequest("Blockchain/MinedBlock", Method.Post);
            request.AddJsonBody(command);


            var response = _Client.Execute(request, Method.Post);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<MinerBlockMinedResponse>(response.Content);
            }

            return null;
        }

        public CertBlock ConvertDtoToBlock(BlockDto block)
        {
            return new CertBlock()
            {
                BlockId = block.BlockId,
                OrderNumber = block.OrderNumber,
                SerialNumber = block.SerialNumber,
                CreatedOn = block.CreatedOn,
                MinedOn = block.MinedOn,
                PreviousHash = block.PreviousHash,
                Nonce = block.Nonce,
                Data = Convert.FromBase64String(block.Data),
                Hash = block.Hash,
                TransactionList = block.TransactionList
            };
        }

        public BlockDto ConvertBlockToDto(CertBlock block)
        {
            return new BlockDto()
            {
                BlockId = block.BlockId,
                OrderNumber = block.OrderNumber,
                SerialNumber = block.SerialNumber,
                CreatedOn = block.CreatedOn,
                MinedOn = block.MinedOn,
                PreviousHash = block.PreviousHash,
                Nonce = block.Nonce,
                Data = Convert.ToBase64String(block.Data),
                Hash = block.Hash,
            };
        }
    }
}
