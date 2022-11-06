using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class RestCallUtil
    {
        public static IEnumerator POSTRequest(string uri, string postData, string token, System.Action<string> callback)
        {


            using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(uri, postData))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(postData);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                webRequest.SetRequestHeader("Content-Type", "application/json");
                if (!string.IsNullOrEmpty(token))
                    webRequest.SetRequestHeader("Authorization", "Bearer " + token);

                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string resultObject = "";

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                if (webRequest.isNetworkError)
                {
                    Debug.Log(pages[page] + ": Error: " + webRequest.error);
                    //PromptMessageController.Controller.ErrorPromptText(pages[page] + ": Error: " + webRequest.error);
                }
                else
                {
                    resultObject = webRequest.downloadHandler.text;
                    callback(resultObject);
                }
            }
        }


        public static IEnumerator GETRequest(string uri, string token, System.Action<string> callback)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Authorization", "Bearer " + token);

                // Request and wait for the desired page.
                yield return ReturnWebRequestResponse(uri, callback, webRequest);

            }
        }


        public static IEnumerator ReturnWebRequestResponse(string uri, System.Action<string> callback, UnityWebRequest webRequest)
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string resultObject = "";

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                // server down, or connection not working
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                resultObject = webRequest.downloadHandler.text;

                callback(resultObject);
            }
        }
    }
}