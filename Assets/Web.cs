using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Web : MonoBehaviour {
    private String _webText;
    private String _webTime;
    private string _timeServer = "http://worldtimeapi.org/api/ip";
    private string _timeForCompare;
    private TimeSpan timeSpan;
    void Start () {

        _timeForCompare = PlayerPrefs.GetString ("ExitTime");

        StartCoroutine (GetRequest (_timeServer));
    }

    IEnumerator GetRequest (string uri) {
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get (uri)) {
            yield return webRequest.SendWebRequest ();

            _webText = webRequest.downloadHandler.text;

            var config = JsonConvert.DeserializeObject<Convert> (_webText);

            _webTime = config.datetime;

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

        Debug.Log ("Количество бонусов раз в 10 сек - " + Math.Floor (timeSpan.TotalSeconds / 10));
        Debug.Log ("Количество бонусов раз в 10 мин - " + Math.Floor (timeSpan.TotalSeconds / 600));
        Debug.Log ("Количество бонусов раз в 1 час - " + Math.Floor (timeSpan.TotalSeconds / 3600));
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