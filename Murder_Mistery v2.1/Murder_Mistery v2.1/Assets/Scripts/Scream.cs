using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scream : MonoBehaviour
{
        [SerializeField]
        float countdown= 1;
        float inizio;
        
        private void Start()
        {
            inizio = -countdown;
        }
        
        /*
        private void Update()
        {
            if(Time.time-inizio>countdown){
                if(screamimage.color==Color.grey)
                {
                    screamimage.color = Color.white;
                }
                if(Input.GetKeyDown(KeyCode.E)){
                    inizio=Time.time;
                    
                    screamimage.color=Color.grey;
                    GetComponent<AudioSource>().Play();
                }
            }
        }
        */

}
