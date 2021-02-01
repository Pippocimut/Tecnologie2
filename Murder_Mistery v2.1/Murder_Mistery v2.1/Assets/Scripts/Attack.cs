using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Gun
{
    // Start is called before the first frame update
    //Arma
    int times = 0;
    bool _Accepted;
    [SerializeField]
    Transform playercamera;
    public override void Shoot(int _id)
    {
        if(transform.childCount>0){
            Destroy(transform.GetChild(0).gameObject);
            ServerSend.DestroyMeshKnife(_id);
            Knife knife = NetworkManager.instance.InstantiateKnife(playercamera,(@"Coltello")).GetComponent<Knife>();
            knife.Initialize(_id);
            ServerSend.SpawnKnife(knife,_id,@"Coltello");
        }
    }
}
