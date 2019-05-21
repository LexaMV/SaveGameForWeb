using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Web : MonoBehaviour {
    private String _webText;
    private String _webTime;
    private string _timeServer = "http://worldtimeapi.org/api/ip";
    private string _timeForCompare;
    private TimeSpan timeSpan;

    public TextMeshProUGUI ExitTime;
    public TextMeshProUGUI StartTime;
    public TextMeshProUGUI BonusSeconds;
    public TextMeshProUGUI BonusMinutes;
    public TextMeshProUGUI BonusHours;
    
    void Start () {

        _timeForCompare = PlayerPrefs.GetString ("ExitTime");

        ExitTime.text = "Время прошлого выхода из игры: " +  _timeForCompare.ToString();

        StartCoroutine (GetRequest (_timeServer));
    }

    IEnumerator GetRequest (string uri) {
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get (uri)) {
            yield return webRequest.SendWebRequest ();

            _webText = webRequest.downloadHandler.text;

            var config = JsonConvert.DeserializeObject<Convert> (_webText);

            _webTime = config.datetime;
            
            StartTime.text = "Время входа в игру: " +  _webTime;

            CompareTime ();
        }
    }

    private void CompareTime () {

        DateTime webTime = DateTime.Parse (_webTime);
        DateTime exitTime = DateTime.Parse (_timeForCompare);
        timeSpan = webTime - exitTime;

        PrintToConsole ();
    }

    private void PrintToConsole () {

        BonusSeconds.text = "Количество бонусов раз в 10 сек - " + Math.Floor (timeSpan.TotalSeconds / 10);
        BonusMinutes.text = "Количество бонусов раз в 10 мин - " + Math.Floor (timeSpan.TotalSeconds / 600);
        BonusHours.text = "Количество бонусов раз в 1 час - " + Math.Floor (timeSpan.TotalSeconds / 3600);
    }

    public void OnApplicationQuit () {

        var exitTime = DateTime.Now.ToString ();
        PlayerPrefs.SetString ("ExitTime", exitTime);
    }
}

[Serializable]
public class Convert {
    public string datetime;
}