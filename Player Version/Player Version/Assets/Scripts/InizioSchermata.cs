using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class InizioSchermata : MonoBehaviour
{   
    public GameObject camera2;
    public AudioSource source;
    public CanvasScaler canvas;

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().isLoaded){
            camera2.SetActive(true);
            source.PlayOneShot(source.clip);
            this.enabled=false;
        }
    }
}
