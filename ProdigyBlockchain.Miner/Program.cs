using ProdigyBlockchain.BusinessLayer;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
 

/// Main Start
/// //////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// 

NodeClient _Client;
IPAddress _ExternalIPAddress;
IPAddress _InternalIPAddress;
int _NetworkDifficulty = 1;

Console.ForegroundColor = ConsoleColor.White;

Console.WriteLine("Miner starting...");

var config = new ConfigurationBuilder().AddJsonFile("config.json", optional: false).Build();
var _Config = new MinerConfig();

config.GetSection("miner").Bind(_Config);

// Checking for general config issues
if (_Config.wallet_address == "" || _Config.wallet_address == "pb09A0A5DC92C4F5F683FA053357209CD773444451AC0")
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("You need to specify your own wallet in the config file.");
    Console.ForegroundColor = ConsoleColor.White;

    return;
}

if (_Config.url == "" || _Config.url == "555.555.555.555")
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("You need to specify a IP address to connect to");
    Console.ForegroundColor = ConsoleColor.White;

    return;
}


// Get address information
GetExternalIP();
GetInternalIP();

Console.WriteLine("Using wallet: " + _Config.wallet_address);


// Setup the client
_Client = new NodeClient(_Config.wallet_address, _InternalIPAddress, _ExternalIPAddress, IPAddress.Parse(_Config.url), _Config.port);

// Get a nodes list and set up a more long term host
var ack_response = _Client.JoinRequest();

if (ack_response != null)
{
    _Client = new NodeClient(_Config.wallet_address, _InternalIPAddress, _ExternalIPAddress, IPAddress.Parse(ack_response.ip_address), _Config.port);
    _NetworkDifficulty = ack_response.difficulty;
}


// Start Mining
while (true)
{
    var rand = new Random((int)DateTime.Now.Ticks);

    var block = _Client.RequestBlock();

    if (block != null)
    {
        WriteToConsole("New job from " + _Config.url + " diff " + block.Nonce + " index " + block.Index + " (" + block.TransactionList.Count() + " tx)", ConsoleColor.DarkMagenta);

        var block_result = block.Mine(_NetworkDifficulty);

        var mined_response = _Client.MinedBlock(block_result.BlockId, block_result.Hash);

        if (mined_response != null && mined_response.validated == true)
        {
            if (mined_response.reward > 0)
            {
                WriteToConsole("Accepted " + block_result.Hash, ConsoleColor.Green);
            }
            else
            {
                WriteToConsole("Accepted " + block_result.Hash, ConsoleColor.Green);
            }
        }
        else
        {
            WriteToConsole("Rejected hash", ConsoleColor.Red);
        }

        Thread.Sleep(rand.Next(1000, 6000));
    }
    else
    {
        Thread.Sleep(10000);
    }

}
// End While
/// Main End
/// //////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// 


void GetExternalIP()
{
    using(var httpClient = new HttpClient())
    {
        var result = httpClient.GetStringAsync("http://icanhazip.com").Result;

        string cleaned = result.Replace("\\r\\n", "").Replace("\\n", "").Trim();

        _ExternalIPAddress = IPAddress.Parse(cleaned);
    }
}

void GetInternalIP()
{
    var host = Dns.GetHostEntry(Dns.GetHostName());
    _InternalIPAddress = host.AddressList.Where(m => m.AddressFamily == AddressFamily.InterNetwork).LastOrDefault();

    if (_InternalIPAddress == null)
    {
        // _InternalIPAddress will be null if running linux so we need to get the ip a different way
        _InternalIPAddress = NetworkInterface.GetAllNetworkInterfaces().SelectMany(i => i.GetIPProperties().UnicastAddresses).Select(a => a.Address).Where(a => a.AddressFamily == AddressFamily.InterNetwork).LastOrDefault();
    }
    else
    {
        // Using windows, we need to get an internal ip by common ip schemes.
        // NOTE:
        // There can be several different adapters and we a re looping through getting "the best match"
        // There is still some potentiality for bugs using this method and will need to be expanded and refined at some point
        var ipAddresses = host.AddressList.Where(m => m.AddressFamily == AddressFamily.InterNetwork).ToList();
        foreach (var theIP in ipAddresses)
        {
            if (theIP.ToString().Contains("192.") || theIP.ToString().Contains("172.") || theIP.ToString().Contains("10."))
            {
                _InternalIPAddress = theIP;
            }
        }
    }

    Console.WriteLine("Binding to: " + _InternalIPAddress.ToString());
}

void WriteToConsole(string message, ConsoleColor color)
{

    Console.Write("[" + DateTime.Now.ToString() + "] ");
    Console.ForegroundColor = color;
    Console.Write(message + "\n");
    Console.ForegroundColor = ConsoleColor.White;
}