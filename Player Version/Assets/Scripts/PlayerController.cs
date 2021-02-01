using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Joystick joystick;
    Shoot_Button shoot_Button;
    Jump_Button jump_Button;
    public Transform camTransform;
    bool shooted=false;
    private void Start()
    {
        joystick = FindObjectOfType<Joystick>();
        shoot_Button = FindObjectOfType<Shoot_Button>();
        jump_Button = FindObjectOfType<Jump_Button>();
    }
    private void FixedUpdate()
    {
        SendInputToServer();
    }
    private void SendInputToServer()
    {
        float[] _axes;
        if(Mathf.Abs(joystick.Vertical)>0.2f || Mathf.Abs(joystick.Horizontal)>0.2f || Mathf.Abs(joystick.Vertical+joystick.Horizontal)>0.2f){
            _axes = new float[]{
                joystick.Vertical,
                joystick.Horizontal
            };
        }
        else{
            _axes = new float[]{0,0};
        }
        
        bool[] _inputs = new bool[]{
            jump_Button.pressed,
            shoot_Button.pressed
        };
        

        ClientSend.PlayerMovement(_axes,_inputs);
    }

}

