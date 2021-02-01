using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Pause : MonoBehaviour
{
    public GameObject panel;
    public void Click(){
        
        Cursor.visible = !Cursor.visible;
        
        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        panel.SetActive(!panel.activeSelf);
    }
    
    public void Resume(){
        Click();
    }
    public void Quit(){
        Client.instance.Disconnect();
        Cursor.lockState = CursorLockMode.None;
        Destroy(Client.instance.gameObject);
        SceneManager.LoadScene(0);
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.O)){
            Click();
        }
    }
}
