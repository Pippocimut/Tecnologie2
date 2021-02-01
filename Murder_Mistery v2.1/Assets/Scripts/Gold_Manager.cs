using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold_Manager : MonoBehaviour
{
    public float countdown=1;
    float now=0;

    void Update()
    {
        if(Time.time-now >= countdown){
            now = Time.time;
            List<GameObject> golds= new List<GameObject>();

            foreach(Gold_Spawner _spawner in GameManager.instance.golds.Values){
                if(!_spawner.transform.GetChild(0).gameObject.activeSelf){
                    golds.Add(_spawner.gameObject);
                }
            }

            if(golds.Count>0){
                int _random = Random.Range(0,golds.Count-1);
                ServerSend.SpawnGold(golds[_random].GetComponent<Gold_Spawner>().id,golds[_random].transform.position);
                golds[_random].transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
    private void OnDestroy()
    {
        Gold_Spawner.nextid =1;
    }
}
