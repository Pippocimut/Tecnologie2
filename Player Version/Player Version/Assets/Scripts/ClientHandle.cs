using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        // Now that we have the client's id, connect UDP
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }
    public static void SpawnPlayer(Packet _packet)
    {
        try{
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        int _type = _packet.ReadInt();
        GameManager.instance.SpawnPlayer(_id, _position, _rotation,_type);
        Debug.Log("Spawn player: "+_id);
        }
        catch{
            Client.instance.Disconnect();
            Debug.Log("Errore sulla creazione");
        }
    }
    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        if (GameManager.instance.players.TryGetValue(_id, out PlayerManager _player))
        {
            _player.transform.position = _position;
        }
    }
    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();

        if (GameManager.instance.players.TryGetValue(_id, out PlayerManager _player))
        {
            _player.transform.rotation = _rotation;
        }
    }
    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Destroy(GameManager.instance.players[_id].gameObject);
        GameManager.instance.players.Remove(_id);
    }
    public static void SpawnKnife(Packet _packet)
    {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        int _thrownByPlayer = _packet.ReadInt();
        Quaternion rotation = _packet.ReadQuaternion();
        GameManager.instance.SpawnKnife(_projectileId,_thrownByPlayer,_position,rotation);
    }
    public static void KnifePosition(Packet _packet)
    {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        if (GameManager.instance.knifes.TryGetValue(_projectileId, out Knife _knife))
        {
            _knife.transform.position = _position;
        }
    }
    public static void DestroyKnife(Packet _packet)
    {
        int _knife = _packet.ReadInt();
        Destroy(GameManager.instance.knifes[_knife].gameObject);
        GameManager.instance.knifes.Remove(_knife);
    }
    public static void DestroyMeshKnife(Packet _packet)
    {
        int _player = _packet.ReadInt();
        GameManager.instance.players[_player].RemoveWeapon();
        
    }
    public static void KnifePickedUp(Packet _packet)
    {
        int _player = _packet.ReadInt();
        GameManager.instance.players[_player].PickUpKnife();
    }
    public static void SpawnKnifeSpawner(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        KnifeSpawner _knifespawner = Instantiate(Resources.Load<GameObject>("items/Drop_object"),_position,_rotation).transform.GetComponent<KnifeSpawner>();
        _knifespawner.Initialize(_id);
    }
    public static void DestroyKnifeSpawner(Packet _packet)
    {
        int _id = _packet.ReadInt();
        GameManager.instance.knifesSpawners[_id].Destroy();
    }
    public static void DestroyPlayer(Packet _packet)
    {   
        int _id = _packet.ReadInt();
        Debug.Log("Destroy player: "+_id);
        try{
            GameManager.instance.players[_id].Destroy();
        }
        catch{
            foreach(int _key in GameManager.instance.players.Keys){
                Debug.Log($"Le chiavi di {_id} : "+_key);
            }
            //Client.instance.Disconnect();
            Debug.Log("Errore sulla distruzione");
        }
    }
    public static void PlayerDie(Packet _packet)
    {
        GameManager.instance.players[_packet.ReadInt()].Die();
    }
    public static void SpawnCorpse(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        GameManager.instance.SpawnCorpse(_id,_position,_rotation);
    }
    public static void UpdatePlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _role = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Debug.Log("Posizione: "+_position);
        GameManager.instance.UpdatePlayer(_id,_role,_position);
        if(_id == Client.instance.myId){
            GameManager.instance.players[_id].uI.ShowRole();
            GameManager.instance.startGame = true;
        }

    }
    public static void Teleport(Packet _packet){
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        GameManager.instance.players[_id].transform.position=_position;
        if(_id == Client.instance.myId){
            GameManager.instance.players[_id].uI.ShowRole();
            GameManager.instance.startGame = true;
        }
    }
    public static void GameBegins(Packet _packet){

        //GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<UI_Player>().ShowRole();
    }
    public static void Gold(Packet _packet){
        GameManager.instance.players[Client.instance.myId].Gold(_packet.ReadInt());
    }
    public static void SpawnGold(Packet _packet){
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        GameManager.instance.SpawnGold(_id,_position);
    }
    public static void DestroyGold(Packet _packet){
        int _id = _packet.ReadInt();
        Debug.Log("Destroy player: "+_id);
        try{
            GameManager.instance.golds[_id].Destroy();
        }
        catch{
            Debug.Log($"Error on destruction of :{_id}");
        }
    }
    public static void SendGold(Packet _packet){
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        GameManager.instance.SpawnGold(_id,_position);
    }
    public static void EndGame(Packet _packet){
        UI_Player.ending = true;
        GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<UI_Player>().End(_packet.ReadBool());
    }
    public static void Shoot(Packet _packet){
        GameManager.instance.players[_packet.ReadInt()].ShootEffect();
    }
    public static void SpawnGhost(Packet _packet)
    {
        try{
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        GameManager.instance.SpawnGhost(_id, _position, _rotation);
        Debug.Log("Spawn player: "+_id);
        }
        catch{
            Client.instance.Disconnect();
            Debug.Log("Errore sulla creazione");
        }
    }
    public static void DestroyGun(Packet _packet)
    {
        GameManager.instance.players[_packet.ReadInt()].DestroyGun();
    }
    public static void SpawnPistol(Packet _packet)
    {
        GameManager.instance.players[_packet.ReadInt()].SpawnPistol();       
    }
    public static void Time(Packet _packet){
        UI_Player.SetTime(_packet.ReadFloat());
    }
}
