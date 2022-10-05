using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using TMPro;
using HtmlAgilityPack;

public class LeaderBoardBrain : MonoBehaviour
{
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

    [Header("")]
    string URL = "https://cy2brmczxr.us-east-1.awsapprunner.com/ranking/";
    public List<User> users;

    //try
    private void Start()
    {
        users = new List<User>();
        ShowScores(users);
    }
    public void SearchButton()
    {
        int group_ = int.Parse(groupText.text);
        users = GetData(group_);
        ShowScores(users);
    }
    List<User> GetData(int round)
    {
        HtmlDocument document;
        HtmlWeb web = new HtmlWeb();
        List<User> scores = new List<User>();

        document = web.Load(URL + round);// + round.ToString());

        for (int i = 1; i <= boradText.childCount; i++)
        {
            try
            {
                User user = new User();

                user.name = document.DocumentNode.SelectNodes("//table/tbody/tr[" + i + "]/td[1]").First().InnerText;
                user.id = document.DocumentNode.SelectNodes("//table/tbody/tr[" + i + "]/td[2]").First().InnerText;
                user.email = document.DocumentNode.SelectNodes("//table/tbody/tr[" + i + "]/td[3]").First().InnerText;
                user.group = document.DocumentNode.SelectNodes("//table/tbody/tr[" + i + "]/td[4]").First().InnerText;              
                user.time = document.DocumentNode.SelectNodes("//table/tbody/tr[" + i + "]/td[5]").First().InnerText;

                scores.Add(user);
            }
            catch (System.Exception)
            {
                break;
            }           
        }
        return scores;
    }
    float TimeToPoints(string time_)
    {
        List<string> timeList = time_.Split(':').ToList();

        float dec = float.Parse(timeList[2]) * 0.01f;
        float sec = float.Parse(timeList[1]);
        float min = float.Parse(timeList[0]) * 60;

        float time = min + sec + dec;
        print(min + " : " + sec + " : " + dec);
        print(time);
        print("-----------------------");

        float points;
        float Vel = trackMeters / time;

        if (Vel > minV)
            points = (((Vel - minV) * (maxP - minP)) / (maxV - minV)) + minP;
        else
            points = (Vel * minP) / minV;

        return points;
    }
    void ShowScores(List<User> users_)
    {
        int points_;
        for (int i = 0; i < boradText.childCount; i++)
        {
            if (i < users_.Count)
            {
                points_ = (int)TimeToPoints(users_[i].time);
                //Debug.Log(scoresToShow[i].name + " | " + points_);

                boradText.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = (i + 1).ToString() + ".";
                boradText.GetChild(i).GetChild(1).GetComponent<TMP_Text>().text = users_[i].name;
                boradText.GetChild(i).GetChild(2).GetComponent<TMP_Text>().text = "#" + users_[i].id;
                boradText.GetChild(i).GetChild(3).GetComponent<TMP_Text>().text = points_.ToString();
                boradText.GetChild(i).GetChild(4).GetComponent<TMP_Text>().text = users_[i].time;
            }
            else
            {
                boradText.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = (i + 1).ToString() + "."; ;
                boradText.GetChild(i).GetChild(1).GetComponent<TMP_Text>().text = "--------";
                boradText.GetChild(i).GetChild(2).GetComponent<TMP_Text>().text = "-";
                boradText.GetChild(i).GetChild(3).GetComponent<TMP_Text>().text = "-";
                boradText.GetChild(i).GetChild(4).GetComponent<TMP_Text>().text = "--:--:--";
            }
        }
    }
    /*
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
        //URL = fakeAPI;
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
                    boradText.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = (i + 1).ToString() + "."; ;
                    boradText.GetChild(i).GetChild(1).GetComponent<TMP_Text>().text = "xxxx";
                    boradText.GetChild(i).GetChild(2).GetComponent<TMP_Text>().text = "#";
                    boradText.GetChild(i).GetChild(3).GetComponent<TMP_Text>().text = "0";
                }
            }
        }
        else
        {
            Debug.Log(www.error);
        }
    }*/
}


[System.Serializable]
public class User
{
    public string name;
    public string id;    
    public string email;
    public string group;
    public string time;
}

