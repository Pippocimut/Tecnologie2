using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);

            SendTCPData(_packet);
        }
    }

    public static void PlayerMovement(float[] _axes,bool[] _inputs)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }

            _packet.Write(_axes.Length);
            foreach (float _axis in _axes)
            {
                _packet.Write(_axis);
            }

            _packet.Write(GameManager.instance.players[Client.instance.myId].transform.rotation);
            _packet.Write(Camera.main.transform.rotation);
            SendUDPData(_packet);
        }

    }

    public static void PlayerShoot()
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerShoot))
        {
            SendTCPData(_packet);
        }
    }

    public static void PlayerThrowItem(Vector3 _facing)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerThrowItem))
        {
            _packet.Write(_facing);

            SendTCPData(_packet);
        }
    }

    public static void Ready(){
        using (Packet _packet = new Packet((int)ClientPackets.ready))
        {
            SendTCPData(_packet);
        }
    }

  
}
