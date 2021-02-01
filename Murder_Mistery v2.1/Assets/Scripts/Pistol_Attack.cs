using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol_Attack : Gun
{
    // Start is called before the first frame update
    public Transform direzione;
    public bool canshoot=true;
    public int shootlimit = -1;
    public float delay=0.5f;
    public float now=0;
    public override void Shoot(int _id)
    {
        if(Time.time-now>=delay){
            if(shootlimit==0){
                ServerSend.DestroyGun(_id);
                gameObject.SetActive(false);
                return;
            }
            now = Time.time;
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(direzione.transform.position,direzione.transform.forward,out hit);
            Debug.Log(hit.transform.tag);
            ServerSend.Shoot(_id);
            if(hit.transform.tag == "Murderer" || hit.transform.tag == "Character"){
                Debug.Log("Ucciso un innocente"+hit.transform.tag);
                if(hit.transform.tag == "Character"){
                    ServerSend.DestroyGun(_id);
                    shootlimit = 0;
                    Debug.Log("Ucciso un innocente");
                    gameObject.SetActive(false);
                    
                }
                hit.transform.GetComponent<Player>().Die();
                
            }
            if(shootlimit-- == 0 && !(shootlimit<0)){
                ServerSend.DestroyGun(_id);
                shootlimit = 1;
                gameObject.SetActive(false);
            }
        }
    }
}
