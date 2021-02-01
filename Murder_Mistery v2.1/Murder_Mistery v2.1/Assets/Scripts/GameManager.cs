using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public int playersNeed = 2;
    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    public Dictionary<int, Knife> knifes = new Dictionary<int, Knife>();
    public Dictionary<int,Gold_Spawner> golds = new Dictionary<int, Gold_Spawner>();
    public Dictionary<int,Coltello_Data> knifesSpawners = new Dictionary<int, Coltello_Data>();
    public Dictionary<int, Corpse> corpses = new Dictionary<int, Corpse>();

    public int nextKnife_id=1;
    public int nextProjectileId = 1;

    public GameObject goldspawners;
    public bool gameStarted=false;
    public bool endgame=false;
    public int seconds;
    public float now;
    public bool starting;
    // Start is called before the first frame update
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
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire1")){
            EndGame();
        }
        if(players.Count>=playersNeed && !starting && !gameStarted){
            starting = true;
            now = Time.time;
        }

        if(starting ){
            ServerSend.Time((int)(seconds-(Time.time-now))*1000);
            if(Time.time-now >=seconds){
            gameStarted=true;
            starting=false;
            GameBegin();
            }
        }

        if(!endgame && gameStarted){
            if(GameObject.FindGameObjectsWithTag("Character").Length==0)
            {
                StartCoroutine(EndGame(true));
            }
            if(GameObject.FindGameObjectsWithTag("Murderer").Length==0){
                StartCoroutine(EndGame());
            }
            
        }
    }

    IEnumerator EndGame(bool _murderWon=false)
    {
        Debug.Log("Ciao");
        ServerSend.EndGame(_murderWon);
        gameStarted = false;
        yield return new WaitForSeconds(1);
        Server.Stop();
        SceneManager.LoadScene(0);
        
    }

    void GameBegin(){

        int _random=0;
        int _random2=0;
        goldspawners.SetActive(true);
        List<int> _players= new List<int>(players.Keys);
        List<GameObject> _spawns = new List<GameObject>(GameObject.FindGameObjectsWithTag("Spawn"));
        List<Player> _players2 = new List<Player>(players.Values);

        
        int _random3 = Random.Range(0,_spawns.Count-1);
        if(Server.clients.Count>=1){
            _random = 0;//Random.Range(0,_players.Count-1);
            Server.clients[_players[_random]].UpdatePlayer(1,_spawns[_random3].transform.position);
            _spawns.Remove(_spawns[_random3]);
            _players2.Remove(Server.clients[_players[_random]].player);
            _players.RemoveAt(_random);
            
            if(Server.clients.Count>=2){
                _random3 = Random.Range(0,_spawns.Count-1);
                _random2 = 0;//Random.Range(0,_players.Count-1);
                Server.clients[_players[_random2]].UpdatePlayer(-1,_spawns[_random3].transform.position);
                _spawns.Remove(_spawns[_random3]);
                _players2.Remove(Server.clients[_players[_random2]].player);
                _players.RemoveAt(_random2);
            }
        }

        foreach(Player _player in _players2){
            _random3 = Random.Range(0,_spawns.Count-1);
            _player.Teleport(_spawns[_random3].transform.position);
            _spawns.Remove(_spawns[_random3]);
        }
    }
    private void OnDestroy()
    {
        instance=null;
    }


}
