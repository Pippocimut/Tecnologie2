using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coltello_Data : MonoBehaviour
{
    
    public int id;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Murderer")
        {
            Transform parento = other.transform;
            Transform parento2 = parento.GetComponent<Player>().weaponManager;
            GameObject instance = Instantiate(Resources.Load<GameObject>(@"Estetico"),parento2.position,parento2.rotation,parento2);
            ServerSend.PickUpKnife(parento.GetComponent<Player>().id);
            ServerSend.DestroyKnifeSpawner(id);
            Destroy(gameObject); 
           
        }
    }
    public void Initialize(){
        id = GameManager.instance.nextKnife_id++;
        ServerSend.SpawnKnifeSpawner(this);
    }
}
