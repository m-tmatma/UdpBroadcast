using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace UdpBroadcast
{
    internal class Program
    {
        static IEnumerable<IPAddress> GetBroadcastAddresses()
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                UnicastIPAddressInformationCollection uniCastCollection = properties.UnicastAddresses;
                if (uniCastCollection != null)
                {
                    foreach(UnicastIPAddressInformation  uniCast in uniCastCollection)
                    {
                        if (uniCast.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            // IP version 4 のアドレス
                            yield return uniCast.Address;
                        }
                    }
                }

            }

            yield break;
        }

        static void SendBroadcastMessage(IPAddress targetAddress, string data)
        {
            // 送信元ポート
            var src_port = 8000;

            // 送信先ポート
            var dst_port = 18000;

            // 送信データ
            var buffer = Encoding.UTF8.GetBytes(data);

            // ブロードキャスト送信
            var client = new UdpClient(src_port);

            // ブロードキャスト有効化
            client.EnableBroadcast = true;
            client.Connect(new IPEndPoint(targetAddress, dst_port));
            client.Send(buffer, buffer.Length);
            client.Close();
        }
        static void Main(string[] args)
        {
            foreach (IPAddress address in GetBroadcastAddresses())
            {
                Console.WriteLine(address);
            }

            var targetAddress = IPAddress.Parse("192.168.11.255");
            SendBroadcastMessage(targetAddress, "Hello, World!");
        }
    }
}