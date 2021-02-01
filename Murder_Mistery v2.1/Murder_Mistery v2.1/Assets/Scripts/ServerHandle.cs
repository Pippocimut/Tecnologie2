using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
    }
    public static void Ready(int _fromClient,Packet _packet){
        Server.clients[_fromClient].SendIntoGame();
    }
    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }

        float[] _axes = new float[_packet.ReadInt()];
        for (int i = 0; i < _axes.Length; i++)
        {
            _axes[i] = _packet.ReadFloat();
        }

        Quaternion _rotation = _packet.ReadQuaternion();
        Quaternion _vision = _packet.ReadQuaternion();
        Debug.Log("Input recieved");
        try{
        Server.clients[_fromClient].player.SetInput(_axes,_inputs, _rotation,_vision);
        }
        catch{
            Debug.Log("Erroroni");
        }
    }
    public static void PlayerShoot(int _fromClient, Packet _packet)
    {
        Server.clients[_fromClient].player.Shoot();
    }
}
