using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MonoBehaviour
{
    public int id;
    public string username;
    public int role = 0;
    public Material colore;
    public void Initialize(Player _player){
        id = _player.id;
        role = _player.role;
        GameManager.instance.corpses.Add(id,this);
    }
}
