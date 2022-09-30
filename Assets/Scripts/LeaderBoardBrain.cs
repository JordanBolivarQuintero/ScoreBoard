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
    [SerializeField] GameObject medals;
    [SerializeField] TMP_InputField groupText;
    [SerializeField] TMP_InputField URLText;

    [Header("")]
    [SerializeField] string URL;
    [SerializeField] string fakeAPI;

    public void SearchButton()
    {
        int group_ = int.Parse(groupText.text);
        StartCoroutine(GetUsers(group_));
    }
    public void ChangeButton()
    {
        URL = URLText.text;
    }
    public void ReturnToFakeAPI()
    {
        URL = fakeAPI;
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
    
    IEnumerator GetUsers(int group_)
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

            scoresToShow = resData.users.Where(x => x.group == group_).ToArray();
            scoresToShow = scoresToShow.OrderBy(x => x.time).ToArray();

            if (scoresToShow == null || scoresToShow.Length < 1)
            {
                medals.SetActive(false);
            }
            else
                medals.SetActive(true);

            int points_;
            for (int i = 0; i < boradText.childCount; i++)
            {
                if (i < scoresToShow.Length)
                {
                    points_ = (int)TimeToPoints(scoresToShow[i].time);
                    //Debug.Log(scoresToShow[i].name + " | " + points_);

                    boradText.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = (i + 1).ToString() + ".";
                    boradText.GetChild(i).GetChild(1).GetComponent<TMP_Text>().text = scoresToShow[i].name;
                    boradText.GetChild(i).GetChild(2).GetComponent<TMP_Text>().text = "#" + scoresToShow[i].id.ToString();
                    boradText.GetChild(i).GetChild(3).GetComponent<TMP_Text>().text = points_.ToString();
                }
                else
                {
                    boradText.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = "";
                    boradText.GetChild(i).GetChild(1).GetComponent<TMP_Text>().text = "";
                    boradText.GetChild(i).GetChild(2).GetComponent<TMP_Text>().text = "";
                    boradText.GetChild(i).GetChild(3).GetComponent<TMP_Text>().text = "";
                }
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
    public string id;
    public int group;
    public string email;
    public float time;
}
[System.Serializable]
public class Scores
{
    public User[] users;
}
