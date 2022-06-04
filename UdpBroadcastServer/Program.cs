using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpBroadcast
{
    internal class Program
    {
        static void Receive1(UdpClient server)
        {
            if (server != null)
            {
                Console.WriteLine("calling ReceiveAsync");
                var result = server.ReceiveAsync();
                Console.WriteLine("returned ReceiveAsync");
                if (result != null)
                {
                    Console.WriteLine("Waiting");
                    result.Wait();
                    Console.WriteLine("returned Wait");
                    Console.WriteLine("Reading => {0}", result.Result.Buffer.Length);
                }
            }
        }


        static void Receive2(UdpClient receivingUdpClient)
        {
            //https://docs.microsoft.com/ja-jp/dotnet/api/system.net.sockets.udpclient.receive?view=net-6.0
            //Creates a UdpClient for reading incoming data.
            //Creates an IPEndPoint to record the IP Address and port number of the sender.
            // The IPEndPoint will allow you to read datagrams sent from any source.
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

                string returnData = Encoding.ASCII.GetString(receiveBytes);

                Console.WriteLine("This is the message you received " +
                                          returnData.ToString());
                Console.WriteLine("This message was sent from " +
                                            RemoteIpEndPoint.Address.ToString() +
                                            " on their port number " +
                                            RemoteIpEndPoint.Port.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            UdpClient receivingUdpClient = new UdpClient(18000);
            while (true)
            {
                Receive1(receivingUdpClient);
                //Receive2(receivingUdpClient);
            }
        }
    }
}