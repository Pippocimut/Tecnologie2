using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shoot_Button : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public bool pressed = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(pressed){
            ClientSend.PlayerShoot();
            pressed = false;
        }
    }
}
