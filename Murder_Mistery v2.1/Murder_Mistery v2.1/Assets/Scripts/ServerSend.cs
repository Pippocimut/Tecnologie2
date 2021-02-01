using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }
    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }
    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);
            _packet.Write(_player.role);

            SendTCPData(_toClient, _packet);
        }
    }
    public static void SpawnGhost(int _toClient, Player _player){
        using (Packet _packet = new Packet((int)ServerPackets.ghostspawn))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);

            SendTCPData(_toClient, _packet);
        }
    }
    public static void PlayerPosition(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);

            SendUDPDataToAll(_packet);
        }
    }
    public static void PlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation);

            SendUDPDataToAll(_player.id, _packet);
        }
    }
    public static void PlayerDisconnected(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet);
        }
    }
    public static void KnifePosition(Knife _projectile)
    {
        using (Packet _packet = new Packet((int)ServerPackets.knifePosition))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);

            SendUDPDataToAll(_packet);
        }
    }
    public static void SpawnKnife(Knife _knife, int _thrownByPlayer,string name)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnKnife))
        {
            _packet.Write(_knife.id);
            _packet.Write(_knife.transform.position);
            _packet.Write(_thrownByPlayer);
            _packet.Write(_knife.transform.rotation);

            SendTCPDataToAll(_packet);
        }
    }
    public static void DestroyMeshKnife(int _id){
        using(Packet _packet = new Packet((int)ServerPackets.destroyMeshKnife))
        {
            _packet.Write(_id);

            SendTCPDataToAll(_packet);
        }
    }
    public static void DestroyKnife(int _id)
    {
        using(Packet _packet = new Packet((int)ServerPackets.destroyKnife))
        {
            _packet.Write(_id);

            SendTCPDataToAll(_packet);
        }
    }
    public static void PickUpKnife(int _id)
    {
        using(Packet _packet = new Packet((int)ServerPackets.pickUpKnife))
        {
            _packet.Write(_id);
            SendTCPDataToAll(_packet);
        }
    }
    public static void DestroyKnifeSpawner(int _id)
    {
        using(Packet _packet = new Packet((int)ServerPackets.destroyKnifeSpawner))
        {
            _packet.Write(_id);
            SendTCPDataToAll(_packet);
        }
    }
    public static void SpawnKnifeSpawner(Coltello_Data _spawner)
    {
        using(Packet _packet = new Packet((int)ServerPackets.spawnKnifeSpawner))
        {
            _packet.Write(_spawner.id);
            _packet.Write(_spawner.transform.position);
            _packet.Write(_spawner.transform.rotation);
            SendTCPDataToAll(_packet);
        }
    }
    public static void DestroyPlayer(Player _player){
        using(Packet _packet = new Packet((int)ServerPackets.destroyPlayer))
        {
            _packet.Write(_player.id);
            SendTCPDataToAll(_packet);
        }
    }
    public static void PlayerDie(Player _player){
        using(Packet _packet = new Packet((int)ServerPackets.playerDie))
        {
            _packet.Write(_player.id);
            SendTCPDataToAll(_packet);
        }
    }
    public static void SpawnCorpse(Corpse _corpse){
       using(Packet _packet = new Packet((int)ServerPackets.spawnCorpse))
        {
            _packet.Write(_corpse.id);
            _packet.Write(_corpse.transform.position);
            _packet.Write(_corpse.transform.rotation);
            SendTCPDataToAll(_packet);
        } 
    }
    public static void UpdatePlayer(int _toClient, Player _player,Vector3 _position)
    {
        using (Packet _packet = new Packet((int)ServerPackets.updatePlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.role);
            _packet.Write(_position);
            Debug.Log("Posizione: "+_position);
            SendTCPData(_toClient, _packet);
        }
    }
    public static void Teleport(int id,Vector3 _position){
        using (Packet _packet = new Packet((int)ServerPackets.teleport))
        {
            _packet.Write(id);
            _packet.Write(_position);
            SendTCPDataToAll(_packet);
        }
    }
    public static void GameBegins(){
        using (Packet _packet = new Packet((int)ServerPackets.gamebegins))
        {
            SendTCPDataToAll(_packet);
        }
    }
    public static void Gold(int _id,int _gold){
        using (Packet _packet = new Packet((int)ServerPackets.gold))
        {
            _packet.Write(_gold);
            SendTCPData(_id,_packet);
        }
    }
    public static void SpawnGold(int _id,Vector3 _position){
        using (Packet _packet = new Packet((int)ServerPackets.spawnGold))
        {
            _packet.Write(_id);
            _packet.Write(_position);
            SendTCPDataToAll(_packet);
        }
    }
    public static void DestroyGold(int _id){
        using (Packet _packet = new Packet((int)ServerPackets.destroyGold))
        {
            _packet.Write(_id);
            SendTCPDataToAll(_packet);
        }
    }
    public static void SendGoldSpawner(int _toClient,int _id){
        using (Packet _packet = new Packet((int)ServerPackets.sendGold))
        {
            _packet.Write(_id);
            _packet.Write(GameManager.instance.golds[_id].transform.position);
            SendTCPData(_toClient,_packet);
        }
    }
    public static void EndGame(bool _murderWon)
    {
        using (Packet _packet = new Packet((int)ServerPackets.endGame))
        {
            _packet.Write(_murderWon);
            SendTCPDataToAll(_packet);
        }
    }
    public static void Shoot(int _id)
    {
        using (Packet _packet = new Packet((int)ServerPackets.shoot))
        {
            _packet.Write(_id);
            SendTCPDataToAll(_packet);
        }
    }
    public static void DestroyGun(int _id)
    {
        using (Packet _packet = new Packet((int)ServerPackets.destroyGun))
        {
            _packet.Write(_id);
            SendTCPDataToAll(_packet);
        }
    }    
    public static void SpawnPistol(int _id)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnGun))
        {
            _packet.Write(_id);
            SendTCPDataToAll(_packet);
        }
    }
    public static void Time(float _time){
        using (Packet _packet = new Packet((int)ServerPackets.time))
        {
            _packet.Write(_time);
            SendUDPDataToAll(_packet);
        }
    }
}
