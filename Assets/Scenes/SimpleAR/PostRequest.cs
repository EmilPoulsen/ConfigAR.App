using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PostRequest : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Upload());
    }

    IEnumerator Upload()
    {   
        WWWForm form = new WWWForm();
        form.AddField("model", "shapemakersample-5");
        form.AddField("points", "[{'x': 1,'y': 0},{'x': 0.5,'y': 0},{'x': 1.5,'y': 1},{'x': -0.5,'y': 1.5}]");

        using (UnityWebRequest www = UnityWebRequest.Post("http://ptsv2.com/t/v416t-1667695141/post", form))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }
}