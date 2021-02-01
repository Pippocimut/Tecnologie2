using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold_Spawner : MonoBehaviour
{
    public static int nextid = 1;
    public int id;
    // Start is called before the first frame update
    private void Awake()
    {
        ThreadManager.ExecuteOnMainThread(() =>
        {
            id = nextid++;
            GameManager.instance.golds.Add(id,this);
        });
        
    }
}
