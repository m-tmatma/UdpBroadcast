using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpBroadcast
{
    internal class Program
    {
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
            var targetAddress = IPAddress.Parse("192.168.11.255");
            SendBroadcastMessage(targetAddress, "Hello, World!");
        }
    }
}