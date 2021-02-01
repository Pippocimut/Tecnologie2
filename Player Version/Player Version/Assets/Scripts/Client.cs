using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;
    public string defaultip = "127.0.0.1";
    public string ip = "127.0.0.1";
    public int port = 26950;
    public int myId = 0;
    public TCP tcp;
    public UDP udp;

    public bool isConnected = false;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
            ip = defaultip;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void OnApplicationQuit()
    {
        Disconnect(); // Disconnect when the game is closed
    }

    public bool ConnectToServer()
    {
        
        tcp = new TCP();
        udp = new UDP();

        InitializeClientData();

        // Connect tcp, udp gets connected once tcp is done
        return tcp.Connect();
    }
#region TCP
    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public bool Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];
            var result = socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
            Debug.Log("Result calculated "+instance.ip+" "+instance.port);
            return socket.Connected;
            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1f));
            if(success){
                return true;
            }
            Debug.Log("Result: Failure");
            Disconnect();
            return false;
        }

        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);
            Client.instance.isConnected = true;
            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();

            receivedData = new Packet();
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null); // Send data to server
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    instance.Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                Disconnect();
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                // If client's received data contains a packet
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    // If packet contains no data
                    return true; // Reset receivedData instance to allow it to be reused
                }
            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                // While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet); // Call appropriate method to handle the packet
                    }
                });

                _packetLength = 0; // Reset packet length
                if (receivedData.UnreadLength() >= 4)
                {
                    // If client's received data contains another packet
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        // If packet contains no data
                        return true; // Reset receivedData instance to allow it to be reused
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true; // Reset receivedData instance to allow it to be reused
            }

            return false;
        }

        public void Disconnect()
        {
            instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }
#endregion
    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        public void Connect(int _localPort)
        {
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet _packet = new Packet())
            {
                SendData(_packet);
            }
        }

        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(instance.myId); // Insert the client's ID at the start of the packet
                if (socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via UDP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (_data.Length < 4)
                {
                    instance.Disconnect();
                    return;
                }

                HandleData(_data);
            }
            catch
            {
                Disconnect();
            }
        }

        private void HandleData(byte[] _data)
        {
            using (Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetId = _packet.ReadInt();
                    packetHandlers[_packetId](_packet); // Call appropriate method to handle the packet
                }
            });
        }

        public void Disconnect()
        {
            instance.Disconnect();

            endPoint = null;
            socket = null;
        }
    }
    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.playerPosition, ClientHandle.PlayerPosition },
            { (int)ServerPackets.playerRotation, ClientHandle.PlayerRotation },
            { (int)ServerPackets.playerDisconnected, ClientHandle.PlayerDisconnected },
            { (int)ServerPackets.spawnKnife, ClientHandle.SpawnKnife },
            { (int)ServerPackets.destroyKnife, ClientHandle.DestroyKnife },
            { (int)ServerPackets.knifePosition, ClientHandle.KnifePosition },
            { (int)ServerPackets.destroyMeshKnife, ClientHandle.DestroyMeshKnife },
            { (int)ServerPackets.pickUpKnife, ClientHandle.KnifePickedUp },
            { (int)ServerPackets.spawnKnifeSpawner, ClientHandle.SpawnKnifeSpawner },
            { (int)ServerPackets.destroyKnifeSpawner, ClientHandle.DestroyKnifeSpawner },
            { (int)ServerPackets.destroyPlayer, ClientHandle.DestroyPlayer },
            { (int)ServerPackets.playerDie, ClientHandle.PlayerDie},
            { (int)ServerPackets.spawnCorpse, ClientHandle.SpawnCorpse},
            { (int)ServerPackets.updatePlayer, ClientHandle.UpdatePlayer},
            { (int)ServerPackets.teleport, ClientHandle.Teleport},
            { (int)ServerPackets.gamebegins, ClientHandle.GameBegins},
            { (int)ServerPackets.gold, ClientHandle.Gold},
            { (int)ServerPackets.spawnGold, ClientHandle.SpawnGold},
            { (int)ServerPackets.destroyGold, ClientHandle.DestroyGold},
            { (int)ServerPackets.sendGold, ClientHandle.SendGold},
            { (int)ServerPackets.endGame, ClientHandle.EndGame},
            { (int)ServerPackets.shoot, ClientHandle.Shoot},
            { (int)ServerPackets.ghostspawn,ClientHandle.SpawnGhost},
            { (int)ServerPackets.destroyGun,ClientHandle.DestroyGun},
            { (int)ServerPackets.spawnGun,ClientHandle.SpawnPistol},
            { (int)ServerPackets.time,ClientHandle.Time},
        };
        Debug.Log("Initialized packets.");
    }
    public void Disconnect()
    {
        if(tcp.socket.Connected){
            if (isConnected)
            {
                Debug.Log("Improvvisazione");
                isConnected = false;
                tcp.socket.Close();
                udp.socket.Close();
                GameManager.instance.endGame=true;
            }
        }
    }

}
