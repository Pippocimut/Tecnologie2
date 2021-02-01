using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    public int id;
    public int thrownByPlayer;
    public void Initialize(int _id,int _thrownByPlayer)
    {
        id = _id;
        thrownByPlayer =_thrownByPlayer;
        gameObject.layer = 0;
    }
}
