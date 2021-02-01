using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Player : MonoBehaviour
{ 
    public Animator animator;
    public Text text;
    public Text won;
    public Text lose;
    public Text time;
    static float seconds=0;
    public static bool ending;

    static public void SetTime(float _time){
        seconds = _time;
    }

    private void Update()
    {
        SetSeconds();
    }

    void SetSeconds(){
        if(seconds<=0 || GameManager.instance.startGame){
            time.text = "";
            return;
        }
        float _minutes = seconds/60;
        float _seconds = seconds%60;
        string _minutesS;
        string _secondsS;
        if(_minutes<10)
            _minutesS = "0"+_minutes;
        else
            _minutesS = _minutes.ToString();
        
        if(_seconds<10)
            _secondsS = "0"+_seconds;
        else
            _secondsS = _seconds.ToString();

        time.text = _minutesS+":"+_secondsS;

    }
    public void ShowRole(){
        StartCoroutine(FadeTextToFullAlpha(2f,0.5f,text));
        animator.SetTrigger("Inizio");
    }     
    
    public IEnumerator FadeTextToFullAlpha(float t,float t2, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
        StartCoroutine(FadeTextToZeroAlpha(t2,i));
    }
 
    public IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator VictoryWait(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
        StartCoroutine(VictoryWaitCount(4f,i));
    }
 
    public IEnumerator VictoryWaitCount(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
        Client.instance.Disconnect();
        ending = false;
    }



    void Win(){
        StartCoroutine(VictoryWait(2f,won));
    }
    void Lose(){
        StartCoroutine(VictoryWait(2f,lose));
    }

    public void End(bool _murderWon){
        if(GameManager.instance.players[Client.instance.myId].role == -1){
            if(_murderWon)
                Win();
            else
                Lose();
        }
        else{
            if(!_murderWon)
                Win();
            else
                Lose();
        }

    }

}
