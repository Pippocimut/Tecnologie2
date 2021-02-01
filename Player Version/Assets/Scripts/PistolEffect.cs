using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolEffect : MonoBehaviour
{   
    public ParticleSystem particle;
    public void Effect(){
        particle.Play();
        //GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip,1f);
    }
}
