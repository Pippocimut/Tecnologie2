using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public Dictionary<int, GoldSpawner> golds = new Dictionary<int, GoldSpawner>();
    public Dictionary<int, Knife> knifes = new Dictionary<int, Knife>();
    public Dictionary<int, KnifeSpawner> knifesSpawners = new Dictionary<int, KnifeSpawner>();
    public Dictionary<int, Corpse> corpses = new Dictionary<int, Corpse>();

    public GameObject[] localPlayer;
    public GameObject[] player;
    public GameObject corpse;
    public GameObject gold;
    public GameObject itemSpawnerPrefab;
    public GameObject projectilePrefab;
    public bool endGame=false;
    public bool startGame=false;


    private void Update()
    {
        
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
        endGame=false;
        players.Clear();
        golds.Clear();
        knifes.Clear();
        knifesSpawners.Clear();
    }
    public void SpawnPlayer(int _id ,Vector3 _position, Quaternion _rotation,int _role)
    {
        GameObject _player;
        if (_id == Client.instance.myId)
        {
            switch(_role){
                case 1:
                    _player = Instantiate(localPlayer[0], _position, _rotation);
                    break;
                case 0:
                    _player = Instantiate(localPlayer[1], _position, _rotation);
                    break;
                case -1:
                    _player = Instantiate(localPlayer[2], _position, _rotation);
                    break;
                default:
                    _player = Instantiate(localPlayer[1], _position, _rotation);
                    break;
            }
        }
        else
        {
            switch(_role){
                case 1:
                    _player = Instantiate(player[0], _position, _rotation);
                    break;
                case 0:
                    _player = Instantiate(player[1], _position, _rotation);
                    break;
                case -1:
                    _player = Instantiate(player[2], _position, _rotation);
                    break;
                default:
                    _player = Instantiate(player[1], _position, _rotation);
                    break;
            }
        }
        _player.GetComponent<PlayerManager>().Initialize(_id);
        try{
            players.Add(_id, _player.GetComponent<PlayerManager>());
        }
        catch{
            players[_id]=_player.GetComponent<PlayerManager>();
        }
    }
    public void SpawnCorpse(int _id,Vector3 _position,Quaternion _rotation)
    {
        Corpse _corpse = Instantiate(corpse,_position,_rotation).transform.GetComponent<Corpse>();
        _corpse.Initialize(_id);
    }
    public void SpawnKnife(int _id,int _thrownByPlayer,Vector3 _position,Quaternion rotation)
    {

        GameObject _knife = Instantiate(Resources.Load<GameObject>(@"Items/Coltello"), _position,rotation);
        _knife.GetComponent<Knife>().Initialize(_id,_thrownByPlayer);
        knifes.Add(_id, _knife.GetComponent<Knife>());
    }
    public void SpawnGold(int _id,Vector3 _position)
    {
        Instantiate(gold,_position,Quaternion.identity).GetComponent<GoldSpawner>().Initialize(_id);
    }
    public void UpdatePlayer(int _id,int _role,Vector3 _position)
    {
        PlayerManager _player = players[_id];
        Quaternion _rotation = _player.transform.rotation;
        players[_id].Destroy();
        //Vector3 _position = _player.transform.position;
        
        GameObject _update;
        if (_id == Client.instance.myId)
        {
            switch(_role){
                case 1:
                    _update = Instantiate(localPlayer[0], _position, _rotation);
                    break;
                case 0:
                    _update = Instantiate(localPlayer[1], _position, _rotation);
                    break;
                case -1:
                    _update = Instantiate(localPlayer[2], _position, _rotation);
                    break;
                default:
                    _update = Instantiate(localPlayer[1], _position, _rotation);
                    break;
            }
        }
        else
        {
            switch(_role){
                case 1:
                    _update = Instantiate(player[0], _position, _rotation);
                    break;
                case 0:
                    _update = Instantiate(player[1], _position, _rotation);
                    break;
                case -1:
                    _update = Instantiate(player[2], _position, _rotation);
                    break;
                default:
                    _update = Instantiate(player[1], _position, _rotation);
                    break;
            }
        }
        _update.GetComponent<PlayerManager>().Initialize(_player.id);
        players[_id]=_update.GetComponent<PlayerManager>();
    }
    public void SpawnGhost(int _id,Vector3 _position, Quaternion _rotation){
        GameObject _player;
        _player = Instantiate(localPlayer[1], _position, _rotation);

        _player.GetComponent<PlayerManager>().InitializeGhost(_id);
        
        try{
            players.Add(_id, _player.GetComponent<PlayerManager>());
        }
        catch{
            players[_id]=_player.GetComponent<PlayerManager>();
        }
    }
}
