using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Siccity.GLTFUtility;

public class ModelLoader : MonoBehaviour
{
    GameObject wrapper;
    string filePath;
    //https://github.com/mrdoob/three.js/raw/dev/examples/models/gltf/DamagedHelmet/glTF/DamagedHelmet.gltf
    //https://raw.githubusercontent.com/KhronosGroup/glTF-Sample-Models/master/2.0/BoxVertexColors/glTF-Embedded/BoxVertexColors.gltf
    //https://github.com/EmilPoulsen/ConfigAR.App/raw/main/boxes.gltf
    //https://configarbackend.azurewebsites.net/configurate

    private void Start()
    {
        filePath = $"{Application.persistentDataPath}/Files/test.gltf";
        //filePath = "https://github.com/EmilPoulsen/ConfigAR.App/raw/main/boxes.gltf";
        wrapper = new GameObject
        {
            name = "Model"
        };
    }

    public void DownloadFile(string url)
    {
        StartCoroutine(GetFileRequest(url, (UnityWebRequest req) =>
        {
            if (req.isNetworkError || req.isHttpError)
            {
                // Log any errors that may happen
                Debug.Log($"{req.error} : {req.downloadHandler.text}");
            }
            else
            {
                // Save the model into our wrapper
                ResetWrapper();
                GameObject model = Importer.LoadFromFile(filePath);
                model.transform.localScale = new Vector3(1f, 1f, 1f);
                //model.transform.SetParent(wrapper.transform);
            }
        }));
    }

    IEnumerator GetFileRequest(string url, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.downloadHandler = new DownloadHandlerFile(filePath);
            yield return req.SendWebRequest();
            callback(req);
        }
    }

void ResetWrapper()
    {
        if (wrapper != null)
        {
            foreach (Transform trans in wrapper.transform)
            {
                Destroy(trans.gameObject);
            }
        }
    }
}

