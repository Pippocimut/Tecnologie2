using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    public int id;
    public CharacterController controller;
    public int gold;
    public int role = 0;
    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 10f;
    public float throwForce = 30f;
    public bool isRunning=false;
    private bool[] inputs;
    private float[] axes;
    private float yVelocity = 0;
    public Transform vision;
    public Transform weaponManager;
    public bool ghost=false;
    public LayerMask deathGround;

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int _id)
    {
        id = _id;
        inputs = new bool[2];
        axes = new float[2];
    }
    
    public void FixedUpdate()
    {
        RaycastHit _hit;
        Physics.Raycast(transform.position,-transform.up,out _hit,Mathf.Infinity,LayerMask.GetMask("Pavimento"));
        Debug.DrawLine(transform.position,_hit.point,Color.red,1f);
        Vector2 _inputDirection = Vector2.zero;
        _inputDirection.y = axes[0];
        _inputDirection.x = axes[1];
        
        Move(_inputDirection);
    }

    private void Move(Vector2 _inputDirection)
    {
        Vector3 _moveDirection= Vector3.zero;
        if(ghost){

            _moveDirection = vision.right * _inputDirection.x + vision.forward * _inputDirection.y;

            if(_moveDirection.y+_moveDirection.x>1){
                _moveDirection.Normalize();
            }
            yVelocity=0;
            if(inputs[1]){
                yVelocity -= moveSpeed ;
            }
            if (inputs[0])
            {
                yVelocity += moveSpeed;
            }
            _moveDirection.y = yVelocity;
        }
        else{
            _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;

            if(_moveDirection.y+_moveDirection.x>1){
                _moveDirection.Normalize();
            }
            if (controller.isGrounded)
            {
                yVelocity = 0f;
                if (inputs[0])
                {
                    yVelocity = jumpSpeed;
                }
            }
            yVelocity += gravity;

            _moveDirection.y = yVelocity;
        }
        _moveDirection *=moveSpeed; 
        controller.Move(_moveDirection);
        ServerSend.PlayerPosition(this);
    }

    public void SetInput(float[] _axes,bool[] _inputs, Quaternion _rotation,Quaternion _vision)
    {
        axes = _axes;
        inputs = _inputs;
        transform.rotation = _rotation;
        vision.rotation = _vision;
        ServerSend.PlayerRotation(this);
    }

    public void Shoot()
    {
        if(weaponManager!=null && weaponManager.childCount>0 && weaponManager.gameObject.activeSelf){
            weaponManager.GetComponent<Gun>().Shoot(id);
        }
    }

    public void GetGold(int _gold){
        gold += _gold;
        if(gold>=3){
            gold = 0;
            if(role!=-1 && !weaponManager.gameObject.activeSelf){
                weaponManager.gameObject.SetActive(true);
                ServerSend.SpawnPistol(id);
            }
        }
        ServerSend.Gold(id,gold);
    }
    public void Die(){
        ghost=true;
        transform.tag= "Ghost";
        gameObject.layer = LayerMask.NameToLayer("Ghost");
        Corpse _corpse = NetworkManager.instance.InstantiateCorpse(this.transform);
        _corpse.Initialize(this);
        ServerSend.SpawnCorpse(_corpse);
        ServerSend.PlayerDie(this);
    }

    public void Teleport(Vector3 _position)
    {
        GetComponent<CharacterController>().enabled=false;
        transform.position=_position;
        ServerSend.Teleport(id,_position);
        GetComponent<CharacterController>().enabled=true;;
    }
}
