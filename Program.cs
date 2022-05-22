﻿using System;
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
        static IPAddress GetBroadcastAddress(IPAddress address, IPAddress mask)
        {
            byte[] bytesAddress = address.GetAddressBytes();
            byte[] bytesMask = mask.GetAddressBytes();
            for( int i = 0; i < bytesAddress.Length; i++ )
            {
                bytesAddress[i] |= (byte)~bytesMask[i];
            }

            return new IPAddress(bytesAddress);
        }


        static IEnumerable<IPAddress> GetBroadcastAddresses()
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                IPInterfaceProperties properties = adapter.GetIPProperties();
                UnicastIPAddressInformationCollection uniCastCollection = properties.UnicastAddresses;
                if (uniCastCollection != null)
                {
                    foreach(UnicastIPAddressInformation  uniCast in uniCastCollection)
                    {
                        if (uniCast.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            IPAddress address = uniCast.Address;
                            if (IPAddress.IsLoopback(address))
                            {
                                continue;
                            }

                            IPAddress broadcast = GetBroadcastAddress(address, uniCast.IPv4Mask);

                            Console.Write($"{adapter.Description} : {adapter.Name} : {broadcast}\n");

                            // IP version 4 のアドレス
                            yield return broadcast;
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
        static Task<int> SendBroadcastMessage(UdpClient client, IPAddress targetAddress, string data)
        {
            // 送信先ポート
            var dst_port = 18000;

            // 送信データ
            var buffer = Encoding.UTF8.GetBytes(data);

            return client.SendAsync(buffer, buffer.Length, new IPEndPoint(targetAddress, dst_port));
        }

        static void SendBroadcastMessageToAll()
        {
            var tasks = new List<Task<int>>();

            var client = new UdpClient();
            // ブロードキャスト有効化
            client.EnableBroadcast = true;

            foreach (IPAddress address in GetBroadcastAddresses())
            {
                // ブロードキャスト送信
                var task = SendBroadcastMessage(client, address, "Hello, World!");

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            client.Close();
        }

        static void Main(string[] args)
        {
            SendBroadcastMessageToAll();
        }
    }
}