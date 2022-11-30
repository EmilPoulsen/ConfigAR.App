using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARSubsystems;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using Siccity.GLTFUtility;
using System.IO;
using System.Text;

namespace UnityEngine.XR.ARFoundation.Samples
{

    /// <summary>
    /// Listens for touch events and performs an AR raycast from the screen touch point.
    /// AR raycasts will only hit detected trackables like feature points and planes.
    ///
    /// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
    /// and moved to the hit position.
    /// </summary>
    [RequireComponent(typeof(ARRaycastManager))]
    [Serializable]
    //Points to send from box instantiation
    public class MyCoordinates
    {
        public float x;
        public float y;
    }
    //Entire object to send to backend
    public class MyClass
    {
        public string model;
        public List<MyCoordinates> points = new List<MyCoordinates>();
    }
    public class PlaceOnPlane : PressInputBase
    {

        string filePath;

        //An instance of the object to send to backend
        MyClass myObject = new MyClass();

        //Dropdown to select the model
        public Dropdown m_Dropdown;
        int m_DropdownValue;
        string m_Message;

        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        GameObject m_PlacedPrefab;

        /// <summary>
        /// The prefab to instantiate on touch.
        /// </summary>
        public Text _title;
        public GameObject placedPrefab
        {
            get { return m_PlacedPrefab; }
            set { m_PlacedPrefab = value; }
        }

        IEnumerator Upload()
        {
            //WWWForm form = new WWWForm();
            //form.AddField("model", "shapemakersample-5");
            //form.AddField("points", "[{'x': 1,'y': 0},{'x': 0.5,'y': 0},{'x': 1.5,'y': 1},{'x': -0.5,'y': 1.5}]"); // https://ptsv2.com/t/v416t-1667695141/post
            string postData = JsonUtility.ToJson(myObject);
            using (UnityWebRequest www = UnityWebRequest.Post("https://configarbackend.azurewebsites.net/configurate",postData))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(postData);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");
                //www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Debug.Log("Form upload complete!");
                    Debug.Log(www.result);
                    Debug.Log(www.downloadHandler.text);
                    //byte[] results = www.downloadHandler.data;
                    //using (var stream = new MemoryStream(results))
                    //using (var binaryStream = new BinaryReader(stream))
                    //{
                    //Debug.Log(binaryStream.ToString());
                    //}
                    // Debug.Log(www.downloadHandler.data.ToString());
                }
            }
        }

        /// <summary>
        /// The object instantiated as a result of a successful raycast intersection with a plane.
        /// </summary>
        public GameObject spawnedObject { get; private set; }

        bool m_Pressed;

        protected override void Awake()
        {
            base.Awake();
            m_RaycastManager = GetComponent<ARRaycastManager>();
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
                    //ResetWrapper();
                    GameObject model = Importer.LoadFromFile(filePath);
                    //Set the scale here... 
                    model.transform.localScale = new Vector3(1f, 1f, 1f);
                    //commented out the code below, seemed to cause GLB to breat (works for GLTF)
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

        private void Start()
        {
            MyCoordinates MyCr = new MyCoordinates();
            MyCr.x = 2;
            MyCr.y = 2;
            myObject.model = "tthackathonofficelayout-4";
            myObject.points.Add(MyCr);

            MyCoordinates MyCr1 = new MyCoordinates();
            MyCr1.x = 0;
            MyCr1.y = 2;
            myObject.points.Add(MyCr1);

            MyCoordinates MyCr2 = new MyCoordinates();
            MyCr2.x = 0;
            MyCr2.y = 0;
            myObject.points.Add(MyCr2);


            MyCoordinates MyCr3 = new MyCoordinates();
            MyCr3.x = 2;
            MyCr3.y = 0;
            myObject.points.Add(MyCr3);


            StartCoroutine(Upload());
       
            filePath = $"{Application.persistentDataPath}/Files/test.gltf";
      
        }

        void Update()
        {

            if (Pointer.current == null || m_Pressed == false)
                return;

            var touchPosition = Pointer.current.position.ReadValue();

            if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                var hitPose = s_Hits[0].pose;
                if (true) 
                {
                    m_DropdownValue = m_Dropdown.value;
                    m_Message = m_Dropdown.options[m_DropdownValue].text;
                    //_title.text = hitPose.position.x.ToString();
                    spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                    MyCoordinates MyCr = new MyCoordinates();
                    MyCr.x = hitPose.position.x;
                    MyCr.y = hitPose.position.z;
                    myObject.model = m_Message;
                    myObject.points.Add(MyCr);
                    string json1 = JsonUtility.ToJson(myObject);
                    _title.text = json1;
                    StartCoroutine(Upload());
                }
            }
        }

        protected override void OnPress(Vector3 position) => m_Pressed = true;

        protected override void OnPressCancel() => m_Pressed = false;

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        ARRaycastManager m_RaycastManager;
    }
}
