using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    public int id;
    public Rigidbody rigidBody;
    public int thrownByPlayer;

    [SerializeField]
    private float speed;
    Rigidbody body ;
    bool distrutto=false;
    bool spawn=false;

    protected void FixedUpdate()
    {
        ServerSend.KnifePosition(this);
    }

    public virtual void Initialize(int _thrownByPlayer)
    {
        id = GameManager.instance.nextProjectileId;
        GameManager.instance.nextProjectileId++;
        GameManager.instance.knifes.Add(id, this);
        rigidBody.isKinematic=false;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rigidBody.AddForce(transform.forward*speed,ForceMode.Impulse);
        thrownByPlayer = _thrownByPlayer;
        spawn=false;
    }

    public void DestroyKnife(){
        GameManager.instance.knifes.Remove(id);
        ServerSend.DestroyKnife(id);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        if(!distrutto){
            distrutto=true;
            Collisione(other);
        }
    }
    void Collisione(Collision other){
        
        if(other.transform.tag == "Character")
        {
            //Spawna la roba
            EffectManager.Velocita(thrownByPlayer);
            other.transform.GetComponent<Player>().Die();
        }
        
        Coltello_Data _knifeSpawner =  Instantiate(Resources.Load<GameObject>(@"Drop_object"),transform.position,Quaternion.identity).GetComponent<Coltello_Data>();
        _knifeSpawner.Initialize();
        DestroyKnife();
                
        /*
        else{
            attaccato=true;
            body.velocity=Vector3.zero;
            body.angularVelocity = Vector3.zero;    
            body.useGravity = false;
            GetComponent<CapsuleCollider>().isTrigger = true;
        }*/
    }
}
