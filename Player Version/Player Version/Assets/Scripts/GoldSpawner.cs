using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldSpawner : MonoBehaviour
{
    public int id;
    public void Initialize(int _id){
        id = _id;
        GameManager.instance.golds.Add(id,this);
    }
    public void Destroy()
    {
        GameManager.instance.golds.Remove(id);
        Destroy(gameObject);
    }
}
