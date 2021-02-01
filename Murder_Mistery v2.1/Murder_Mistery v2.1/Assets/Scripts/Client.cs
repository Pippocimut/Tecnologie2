using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client
{
    //Dimensione Buffer Lettura/Scrittura
    public static int dataBufferSize = 4096;
    //ID di riconoscimento
    public int id;
    //Istanza del Player Server
    public Player player;
    //Classi Invio/Ricezione Dati
    public TCP tcp;
    public UDP udp;
    //Costruttore
    public Client(int _clientId)
    {
        id = _clientId;
        //Assegnazione ID per Debug
        tcp = new TCP(id);
        udp = new UDP(id);
    }
    //Definizione Classi TCP/UDP
    public class TCP
    {
        public TcpClient socket=null;

        private readonly int id;
        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public TCP(int _id)
        {
            id = _id;
        }
        public void Connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;

            stream = socket.GetStream();

            receivedData = new Packet();
            receiveBuffer = new byte[dataBufferSize];

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            ServerSend.Welcome(id, "Welcome to the Pippo!");
        }
        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null); // Send data to appropriate client
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to player {id} via TCP: {_ex}");
            }
        }
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    Server.clients[id].Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error receiving TCP data: {_ex}");
                Server.clients[id].Disconnect();
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
                        Server.packetHandlers[_packetId](id, _packet); // Call appropriate method to handle the packet
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
            socket.Close();
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }
    public class UDP
    {
        //IP destinatario
        public IPEndPoint endPoint;
        //ID Mittente
        private int id;
        //Costruttore e Assegnazione IDCLient
        public UDP(int _id)
        {
            id = _id;
        }
        //Assegnazione ednPoint
        public void Connect(IPEndPoint _endPoint)
        {
            endPoint = _endPoint;
        }
        //Invio Dati
        public void SendData(Packet _packet)
        {
            Server.SendUDPData(endPoint, _packet);
        }
        //Analisi Pachetti
        public void HandleData(Packet _packetData)
        {
            int _packetLength = _packetData.ReadInt();
            byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

            //Eseguita in maniera Asincrona
            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_packetBytes))
                {
                    int _packetId = _packet.ReadInt();
                    Server.packetHandlers[_packetId](id, _packet); // Call appropriate method to handle the packet
                }
            });
        }
        //Azzeramento endPoint
        public void Disconnect()
        {
            endPoint = null;
        }
    }
    //Funzione usata per distrugegre il player
    public void DestroyPlayer()
    {
        UnityEngine.Object.Destroy(player.gameObject);
    }
    //Invio Player ai Client 
    public void SendIntoGame()
    {
        if(GameManager.instance.players.Count>Server.MaxPlayers)
        {
            
        }
        else
        {
            player = NetworkManager.instance.InstantiatePlayer();
            player.Initialize(id);
            // Send all players to the new player
            foreach (Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    if (_client.id != id)
                    {
                        ServerSend.SpawnPlayer(id, _client.player);
                    }
                }
            }

            if(GameManager.instance.gameStarted){
                player.ghost=true;
                player.transform.tag= "Ghost";
                player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                ServerSend.SpawnGhost(id, player);
            }
            else{
                

                Debug.Log($"Spawning{id} ");
                // Send the new player to all players (including himself)
                foreach (Client _client in Server.clients.Values)
                {
                    if (_client.player != null)
                    {
                        ServerSend.SpawnPlayer(_client.id, player);
                    }
                }

                foreach (Gold_Spawner _spawner in GameManager.instance.golds.Values)
                {
                    ServerSend.SendGoldSpawner(id, _spawner.id);
                }
            }
            GameManager.instance.players.Add(id,player);
        }
        
    }
    //Modifica Ruolo di un player
    public void UpdatePlayer(int _newrole,Vector3 _position)
    {
        Debug.Log("I'm cancelling"+player.id);
        DestroyPlayer();
        switch(_newrole){
            case -1:
                player = NetworkManager.instance.InstantiateMurder();
                break;
            case  0:
                player = NetworkManager.instance.InstantiatePlayer();
                break;
            case  1:
                player = NetworkManager.instance.InstantiateDetective();
                break;
        }
        player.Initialize(id);
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = _position;
        player.GetComponent<CharacterController>().enabled = true;
        Debug.Log("Vera posizione: "+player.transform.position);
        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                ServerSend.UpdatePlayer(_client.id,player,_position);
            }
        }

        GameManager.instance.players[id]=player;
    }
    //Disconnessione Client
    public void Disconnect()
    {
        if(tcp.socket != null)
        {
            try{
            Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");
            }
            catch{

            }
            try{
            ThreadManager.ExecuteOnMainThread(()=>{
                GameManager.instance.players.Remove(player.id);
                DestroyPlayer();
                player = null;
            });}
            catch{

            }
            try{
            tcp.Disconnect();
            udp.Disconnect();

            ServerSend.PlayerDisconnected(id);
            }
            catch(Exception _ex)
            {
                Debug.Log($"Disconnessione Improvvisa Errore:{_ex}");
            }
        }
    }
}
