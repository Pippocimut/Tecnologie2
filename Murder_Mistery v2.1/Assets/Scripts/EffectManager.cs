using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    static public void Velocita(int _id){
        Transform _owner = GameManager.instance.players[_id].transform;
        Instantiate(Resources.Load<GameObject>(@"Effetti/Velocizza"),_owner.position,Quaternion.identity,_owner);
    }
}
