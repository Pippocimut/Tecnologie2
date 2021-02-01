using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MonoBehaviour
{
    


    public int id;
    public string username;
    public int role = 0;
    public Material colore;
    public void Initialize(int _id){
        id = _id;
        GameManager.instance.corpses.Add(id,this);
        //username = _username;
        //role = _role;
    }
}
