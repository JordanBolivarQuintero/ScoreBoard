using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using TMPro;

public class LeaderBoardBrain : MonoBehaviour
{
    User[] scoresToShow;

    [Header("Reference Values")]
    [SerializeField] float minV = 2.78f;//m/s
    [SerializeField] float maxV = 12.5f;//m/s
    [SerializeField] float minP = 100;
    [SerializeField] float maxP = 1000;
    [SerializeField] float trackMeters = 4000; //meters

    [Header("Text fields")]
    [SerializeField] Transform boradText;

    [Header("")]
    [SerializeField] string URL;

    private void Start()
    {
        StartCoroutine(GetUsers());
    }
    float TimeToPoints(float time)
    {
        float points;
        float Vel = trackMeters/time;

        if (Vel > minV)
            points = (((Vel - minV) * (maxP - minP)) / (maxV - minV)) + minP;
        else
            points = (Vel * minP) / minV;

        return points;
    }
    
    IEnumerator GetUsers()
    {
        string url = URL + "/scores";
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR " + www.error);
        }
        else if (www.responseCode == 200)
        {
            //Debug.Log(www.downloadHandler.text);
            Scores resData = JsonUtility.FromJson<Scores>(www.downloadHandler.text);
            scoresToShow = resData.users.OrderBy(x => x.time).ToArray();

            int points_;
            for (int i = 0; i < 3; i++)
            {               
                points_ = (int)TimeToPoints(scoresToShow[i].time);
                //Debug.Log(scoresToShow[i].name + " | " + points_);
                boradText.GetChild(i).gameObject.GetComponent<TMP_Text>().text = scoresToShow[i].name;
                boradText.GetChild(i + 3).gameObject.GetComponent<TMP_Text>().text = points_.ToString();
            }
        }
        else
        {
            Debug.Log(www.error);
        }
    }
}


[System.Serializable]
public class User
{
    public string name;
    public int id;
    public string email;
    public float time;
}
[System.Serializable]
public class Scores
{
    public User[] users;
}
