using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System;

public class UIManager : MonoBehaviour
{
    public Animator animator;
    public GameObject uno;
    public GameObject due;
    public static UIManager instance;
    public GameObject startMenu;
    public CanvasScaler canvas;
    public GameObject options;

    public InputField input;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            
            Destroy(this);
        }
    }

    public void Option(){
        options.SetActive(!options.activeSelf);
        startMenu.SetActive(!startMenu.activeSelf);
        uno.SetActive(!uno.activeSelf);
        due.SetActive(!due.activeSelf);
    }

    public void Default(){
        Client.instance.ip = Client.instance.defaultip;
        Option();
    }

    public void Salva(){
        if(input.text!=null && input.text.Trim()!=""){
            try{
                IPAddress.Parse(input.text);
            }
            catch(FormatException _ex){
                input.text = _ex.ToString();
                return;
            }
            Client.instance.ip = input.text ;
        }
        Option();
    }

    public void ConnectToServer()
    {
        if(!Client.instance.ConnectToServer())
        {
            Debug.Log("Connection Failed no Server found");
        };
        StartCoroutine(Waitforconnection());
        
    }

    IEnumerator Waitforconnection(){
        while(!Client.instance.isConnected){
            yield return new WaitForSeconds(0.03f);
        }
        Cursor.visible = false;
        Floating _one = uno.GetComponent<Floating>();
        Floating _two = due.GetComponent<Floating>();  
        _one.degreesPerSecond = _two.degreesPerSecond = 1500;
        _one.amplitude = _two.amplitude = 0.2f;
        Animazioni();
        
    }

    void Animazioni(){
        animator.SetTrigger("Connesso");
    }
    public void NuovaScena(){
        SceneManager.LoadScene(1);
    }
}
