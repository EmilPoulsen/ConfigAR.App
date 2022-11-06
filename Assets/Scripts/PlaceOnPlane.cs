using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using System;

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
    public class MyClass
    {
        public int level;
        public float timeElapsed;
        public string playerName;
    }

    public class PlaceOnPlane : MonoBehaviour
    {

        
        MyClass myObject = new MyClass();
        
        


        public Dropdown m_Dropdown;
        //This is the string that stores the current selection m_Text of the Dropdown
        string m_Message;
        //This Text outputs the current selection to the screen
        //public Text m_Text;
        //This is the index value of the Dropdown
        int m_DropdownValue;


        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        GameObject m_PlacedPrefab;

        /// <summary>
        /// The prefab to instantiate on touch.
        /// </summary>
        ///

        public Text _title;
        //find your dropdown object
       

        public GameObject placedPrefab
        {
            get { return m_PlacedPrefab; }
            set { m_PlacedPrefab = value; }
        }

        /// <summary>
        /// The object instantiated as a result of a successful raycast intersection with a plane.
        /// </summary>
        public GameObject spawnedObject { get; private set; }

        void Awake()
        {
            m_RaycastManager = GetComponent<ARRaycastManager>();
        }

        void Start()
        {
            StartCoroutine(Upload());
            myObject.level = 1;
            myObject.timeElapsed = 47.5f;
            myObject.playerName = "Dr Charles Francis";
            string json = JsonUtility.ToJson(myObject);
            Debug.Log(json);
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

        bool TryGetTouchPosition(out Vector2 touchPosition)
        {
            if (Input.touchCount > 0)
            {
                touchPosition = Input.GetTouch(0).position;
                return true;
            }

            touchPosition = default;
            return false;
        }

        void Update()
        {
            if (!TryGetTouchPosition(out Vector2 touchPosition))
                return;

            if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                var hitPose = s_Hits[0].pose;
                //Input.GetTouch(0).phase == TouchPhase.Ended
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    //Keep the current index of the Dropdown in a variable
                    m_DropdownValue = m_Dropdown.value;
                    //Change the message to say the name of the current Dropdown selection using the value
                    m_Message = m_Dropdown.options[m_DropdownValue].text;
                    //Change the onscreen Text to reflect the current Dropdown selection
                    //m_Text.text = m_Message;

                    spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                    //Debug.Log(hitPose.position.x);
                    _title.text += m_Message + ", " + hitPose.position.x.ToString() + ", " + hitPose.position.z.ToString();

                    //if (spawnedObject == null)
                    //{
                    //    spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                    //    Debug.Log(hitPose.position.x);
                    //    _title.text = hitPose.position.x.ToString() + ", " + hitPose.position.z.ToString();
                    //}
                    //else
                    //{
                    //    spawnedObject.transform.position = hitPose.position;
                    //    Debug.Log(hitPose.position.x);
                    //    _title.text = hitPose.position.x.ToString() + ", " + hitPose.position.z.ToString();
                    //}
                }
            }
        }

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        ARRaycastManager m_RaycastManager;
    }
}


