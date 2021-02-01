using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    public Transform playerbody;
    // Player setting
    public float cameraSensitivity;

    // Touch detection
    int FingerId;
    int FingerShoot;
    // Camera control
    Vector2 lookInput;
    float cameraPitch;
    // Player movement
    Vector2 moveTouchStartPosition;
    public Transform[] limits;
    // Start is called before the first frame update
    void Start()
    {
        // id = -1 means the finger is not being tracked
        FingerId = -1;
        //Cursor.lockState = CursorLockMode.Locked;
    }
    void GetTouchInput() {

            Touch[] ts = Input.touches;
            foreach (Touch t in ts){
            // Check each touch's phase
            switch (t.phase)
            {
            case TouchPhase.Began:
            if(FingerId==-1){
                if(!(t.position.y<limits[0].position.y && t.position.x<limits[1].position.x)){
                    FingerId = t.fingerId;
                }
            }/*
            else if (FingerShoot==-1){
                FingerShoot = t.fingerId;
                ClientSend.PlayerShoot();
            }*/
                break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:

                if (t.fingerId == FingerId)
                {
                    // Stop tracking the left finger
                    FingerId = -1;
                   
                    Debug.Log("Stopped tracking finger");
                }/*
                else if(t.fingerId == FingerShoot){
                    FingerShoot = -1;
                }*/
                break;
            case TouchPhase.Moved:

                    // Get input for looking around
                if (t.fingerId == FingerId)
                {
                    lookInput = t.deltaPosition * cameraSensitivity * Time.deltaTime;
                }

                break;
            case TouchPhase.Stationary:
                    // Set the look input to zero if the finger is still
                if (t.fingerId == FingerId)
                {
                    lookInput = Vector2.zero;
                }
                break;
        }
        }
    }

    void LookAround() {

        // vertical (pitch) rotatio 
        cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, -90f, 90f);
        transform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        // horizontal (yaw) rotation
        transform.Rotate(transform.up, lookInput.x);
    }


    float xRotation = 0f;

    void Update()
    {
        
        // Handles input
        GetTouchInput();
        if (FingerId != -1) {
            // Ony look around if the right finger is being tracked
            Debug.Log("Rotating");
            //LookAround();
        

        float mouseY = lookInput.y;//Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        float mouseX = lookInput.x;//Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        xRotation-=mouseY;
        xRotation = Mathf.Clamp(xRotation,-85,85);
        transform.localRotation = Quaternion.Euler(xRotation,0f,0f);
        playerbody.Rotate(Vector3.up*mouseX);
        
        }
    }
}
