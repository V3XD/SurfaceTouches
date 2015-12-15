/*
 * Adapted from Matt Oskamp
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace SurfaceManagement
{
    public static class UDPClient
    {
        private static UdpClient client;
        private static int dataPort = 11000;
        private static DataStream dataStream = null;
        private static bool isListening = false;

        public static void Close()
        {
            if (client != null)
            {
                isListening = false;
                client.Close();
            }
        }

        public static DataStream GetDataStream()
        {
            return dataStream;
        }

        public static bool IsListening()
        {
            return isListening;
        }

        public static void Start()
        {
            StartClient();
        }

        // Unpack data
        private static void ProcessMessage(string message)
        {
            try
            {
                string[] info = message.Split(',');
                string function = info[0];
                Touch touch = new Touch(int.Parse(info[1]), int.Parse(info[2]), int.Parse(info[3]));

                if (function == "OnTouchDown")
                {
                    OnTouchDown(touch);
                }
                else if (function == "OnTouchUp")
                {
                    OnTouchUp(touch);
                }
                else
                {
                    if (dataStream.TouchMove.ContainsKey(touch.getID())) // Update touch position
                    {
                        dataStream.TouchMove[touch.getID()] = touch;
                    }
                    else // We missed the OnTouchDown message
                    {
                        OnTouchDown(touch);
                        Debug.Log("missed touchdown");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private static void OnTouchDown(Touch touch)
        {
            if (!dataStream.TouchMove.ContainsKey(touch.getID()))
            {
                dataStream.TouchMove.Add(touch.getID(), touch);
                dataStream.TouchDown.Enqueue(touch);
                dataStream.IDlist.Add(touch.getID());
            }
        }

        public static void OnTouchUp(Touch touch)
        {
            dataStream.TouchUp.Enqueue(touch);
            dataStream.IDlist.Remove(touch.getID());
        }

        private static void ReadPacket(Byte[] data)
        {
            string message = Encoding.UTF8.GetString(data);
            if (message != "exit")
            {
                ProcessMessage(message);
            }
            else
            {
                Close();
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                UdpState state = (UdpState)ar.AsyncState;
                IPEndPoint ep = state.endPt;
                UdpClient client = state.client;

                if (isListening)
                {
                    // Read data from the remote device.
                    Byte[] receiveBytes = client.EndReceive(ar, ref ep);
                    ReadPacket(receiveBytes);
                }

                if (isListening)
                {
                    client.BeginReceive(new AsyncCallback(ReceiveCallback), state);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private static void StartClient()
        {
            // Connect to a remote device.
            try
            {
                Debug.Log("Starting UDPclient");
                dataStream = new DataStream();
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, dataPort);
                client = new UdpClient(ep);
                isListening = StartReceiver(client, ep);
            }
            catch (Exception e)
            {
                Debug.LogError("UDPclient: " + e.ToString());
            }
        }

        private static bool StartReceiver(UdpClient client, IPEndPoint ep)
        {
            try
            {
                UdpState state = new UdpState();
                state.endPt = ep;
                state.client = client;

                client.BeginReceive(new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return false;
            }

            return true;
        }
    }

    internal class UdpState
    {
        public UdpClient client;
        public IPEndPoint endPt;
    }
}