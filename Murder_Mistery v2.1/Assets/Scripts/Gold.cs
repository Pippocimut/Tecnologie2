using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag=="Character" || other.transform.tag == "Murderer")
        {
            other.transform.GetComponent<Player>().GetGold(1);
            ServerSend.DestroyGold(transform.parent.GetComponent<Gold_Spawner>().id);
            gameObject.SetActive(false);
        }
    }
}
