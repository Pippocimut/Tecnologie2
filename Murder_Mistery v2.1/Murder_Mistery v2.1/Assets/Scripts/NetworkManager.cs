using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public int port=26950;
    public int MaxPlayers=10;
    public GameObject playerPrefab;
    public GameObject murderPrefab;
    public GameObject detectivePrefab;
    public GameObject knifePrefab;
    public GameObject corpse;

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

    private void OnDestroy()
    {
        if(instance = this){
            instance=null;
        }
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        Debug.Log("Staring");
        Server.Start(MaxPlayers, port);
    }

    private void OnApplicationQuit(){
        Server.Stop();
    }
    public Player InstantiatePlayer(){
        return Instantiate(playerPrefab, new Vector3(0f, 0.5f, 0f), Quaternion.identity).GetComponent<Player>();
    }
     public Player InstantiateMurder(){
        return Instantiate(murderPrefab, new Vector3(0f, 0.5f, 0f), Quaternion.identity).GetComponent<Player>();
    }
     public Player InstantiateDetective(){
        return Instantiate(detectivePrefab, new Vector3(0f, 0.5f, 0f), Quaternion.identity).GetComponent<Player>();
    }
    public GameObject InstantiateKnife(Transform _shootOrigin,string name){
        return Instantiate(Resources.Load<GameObject>(name), _shootOrigin.position,_shootOrigin.rotation);
    }
    public Corpse InstantiateCorpse(Transform _player)
    {  
        Vector3 _position= Vector3.zero;
        if(Physics.Raycast(_player.position,-_player.up, out RaycastHit _hit,Mathf.Infinity,LayerMask.GetMask("Pavimento"))){
            _position = new Vector3(_hit.point.x,_hit.point.y+0.25f,_hit.point.z);
            Debug.Log("Trovato terreno");
        }
        return Instantiate(corpse,_position,_player.rotation).GetComponent<Corpse>();
    }
    
}
