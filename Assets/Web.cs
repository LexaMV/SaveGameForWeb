using System;
using System.Collections;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Web : MonoBehaviour {

    private String _webText;
    private String _unixTime;
    private String _utcSeconds;
    private string _timeServer = "http://worldtimeapi.org/api/ip";
    private string _timeExit;
    private Double timeSpan;
    public TextMeshProUGUI ExitTime;
    public TextMeshProUGUI StartTime;
    public TextMeshProUGUI BonusSeconds;
    public TextMeshProUGUI BonusMinutes;
    public TextMeshProUGUI BonusHours;

    void Start() {

        _timeExit = PlayerPrefs.GetString("ExitTime");

        StartCoroutine(GetRequest(_timeServer));
    }

    IEnumerator GetRequest(string uri) {

        using(UnityWebRequest webRequest = UnityWebRequest.Get(uri)) {

            yield return webRequest.SendWebRequest();

            _webText = webRequest.downloadHandler.text;

            var config = JsonConvert.DeserializeObject<Convert>(_webText);

            _unixTime = config.unixtime;

            _utcSeconds = config.raw_offset;

            var unixTimeDouble = System.Convert.ToDouble(_unixTime);
            
            var timeZoneDouble = System.Convert.ToDouble(_utcSeconds);

            var timeExitDouble = System.Convert.ToDouble(_timeExit);

            StartTime.text = "Время входа в игру: " + UnixTimestampToDateTime(unixTimeDouble + timeZoneDouble);

            ExitTime.text = "Время прошлого выхода из игры: " + UnixTimestampToDateTime(timeExitDouble + timeZoneDouble);

            CompareTime();
        }
    }



    private void CompareTime() {

        timeSpan = Double.Parse(_unixTime) - Double.Parse(_timeExit);
        PrintToConsole();
    }

    private void PrintToConsole() {

        BonusSeconds.text = "Количество бонусов раз в 10 сек - " + Math.Floor(timeSpan / 10);
        BonusMinutes.text = "Количество бонусов раз в 10 мин - " + Math.Floor(timeSpan / 600);
        BonusHours.text = "Количество бонусов раз в 1 час - " + Math.Floor(timeSpan / 3600);
    }

    public void OnApplicationQuit() {

        StartCoroutine(GetRequest(_timeServer));
        PlayerPrefs.SetString("ExitTime", _unixTime);
        Application.Quit();
    }

    public DateTime UnixTimestampToDateTime(double unixTime) {

        DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        long unixTimeStampInTicks = (long) (unixTime * TimeSpan.TicksPerSecond);
        return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
    }

    public double DateTimeToUnixTimestamp(DateTime dateTime) {
        
        DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        long unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixStart).Ticks;
        return (double) unixTimeStampInTicks / TimeSpan.TicksPerSecond;
    }
}

[Serializable]
public class Convert {
    public string unixtime;
    public string raw_offset;
}