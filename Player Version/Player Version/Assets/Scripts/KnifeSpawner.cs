using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeSpawner : MonoBehaviour
{
    public int id;
    public void Initialize(int _id)
    {
        id = _id;
        GameManager.instance.knifesSpawners.Add(id,this);
    }

    public void Destroy(){
        Destroy(gameObject);
    }
}
