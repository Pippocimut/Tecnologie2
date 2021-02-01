using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed=1f;
    private Vector3 velocity;
    [SerializeField]
    private bool isGrounded=true;
    private CharacterController controller;
    [SerializeField]
    private Transform feet;
    [SerializeField]
    float jumpheight= 1;
    [SerializeField]
    private float grounD= 0.3f;
    [SerializeField]
    private LayerMask groundMask;
    // Start is called before the first frame update
    void Start()
    {
        controller = transform.GetComponent<CharacterController>();
    }
    
    // Update is called once per frame
    void Update()
    {
        velocity += Physics.gravity*Time.deltaTime;
        Move();
        Jump();
        controller.Move(velocity*Time.deltaTime);
    }
    void Jump(){
        if(isGrounded){
            if(velocity.y<Physics.gravity.y){
                velocity.y=-2;
            }
            if(Input.GetButtonDown("Jump")){
                Debug.Log("Sorcio");
                    velocity.y = Mathf.Sqrt(2 * jumpheight * -Physics.gravity.y);
            }
        }
    }
    void Move(){
        isGrounded = Physics.Raycast(feet.position,-transform.up,grounD,groundMask);
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right*x+transform.forward*z;
        if(move.magnitude>1){
            move.Normalize();
        }
        controller.Move(move*speed*Time.deltaTime);
    }
}
