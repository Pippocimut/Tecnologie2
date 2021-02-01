using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lock : MonoBehaviour
{    void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex == 1 && SceneManager.GetActiveScene().isLoaded){
            ClientSend.Ready();
            this.enabled=false;
        }
    }
}
