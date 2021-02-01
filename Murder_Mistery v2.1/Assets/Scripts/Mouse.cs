using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class Mouse : MonoBehaviour
{
    [SerializeField]
    private float mouseSensitivity = 100f;
    public Transform playerbody;
    float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") *mouseSensitivity * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity*Time.deltaTime;
        xRotation-=mouseY;
        xRotation = Mathf.Clamp(xRotation,-85,95);
        transform.localRotation = Quaternion.Euler(xRotation,0f,0f);
        playerbody.Rotate(Vector3.up*mouseX);
    }
}*/
