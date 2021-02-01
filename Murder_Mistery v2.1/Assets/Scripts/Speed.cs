using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : MonoBehaviour
{
    [SerializeField]
    float speed = 2f;
    float timeadesso;
    float durata=3;
    // Start is called before the first frame update
    void Start()
    {
        transform.parent.GetComponent<Player>().moveSpeed*=speed;
        timeadesso = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time-timeadesso>durata){
            transform.parent.GetComponent<Player>().moveSpeed/=speed;
            Destroy(gameObject);
        }
    }
}
