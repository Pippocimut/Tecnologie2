using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Scene_Manager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex !=0 && GameManager.instance.endGame && !UI_Player.ending)
        {
            Cursor.lockState = CursorLockMode.None;
            Destroy(Client.instance.gameObject);
            SceneManager.LoadScene(0);
        }
    }
}
