using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractGraph : MonoBehaviour {
    public int maxMarkCount;
    [HideInInspector]
    public float doubleTapTime = 1;
    private float timeBeforeNextTap;
    [HideInInspector]
    public bool drawn = false;
    [HideInInspector]
    public GameObject panel, camera;
    [HideInInspector]
    public bool detailed = false;
    public float distanceToCamera = 10;
    [HideInInspector]
    public List<Link> links = new List<Link> ();
    [HideInInspector]
    public GameObject[] dots;
    [HideInInspector]
    public Vector3 navigationHandStartPosition;
    [HideInInspector]
    public Vector3 navigationStartPosition;
    public int movementSensibility = 2;

    private Vector3 oldPosition, oldRotation;

    public virtual void Start()
    {
        panel = transform.Find("Graph").gameObject;
        camera = Camera.main.gameObject;
        Draw(GameObject.Find("CSV").GetComponent<CSV>().data);
        timeBeforeNextTap = 0;
        oldPosition = GetComponent<RectTransform>().localPosition;
        oldRotation = GetComponent<RectTransform>().eulerAngles;
    }

    void Update()
    {
        if (detailed)
        {
            transform.forward = camera.transform.forward;
            transform.position = camera.transform.position + camera.transform.forward * distanceToCamera;
            
            //drawing links
            foreach (Link link in links)
            {
                link.Draw(false);
            }
        }
        //decreasing timeBeforeNextTap
        timeBeforeNextTap = timeBeforeNextTap <= 0 ? 0 : timeBeforeNextTap - Time.deltaTime; 
    }

    public virtual void Draw(List<Dictionary<string, string>> data)
    {
    }

    public void OnSelect()
    {
        //double tap
        if (timeBeforeNextTap > 0)
        {
            //start detailed mode
            if (!detailed)
            {
                oldPosition = GetComponent<RectTransform>().localPosition;
                oldRotation = GetComponent<RectTransform>().eulerAngles;
                timeBeforeNextTap = 0;
                print("Select " + name);
            }

            //stop detailed mode
            else
            {
                print("Deselct " + name);
                GetComponent<RectTransform>().localPosition = oldPosition;
                GetComponent<RectTransform>().eulerAngles = oldRotation;
                
                //drawing links
                foreach (Link link in links)
                {
                    link.Draw(false);
                }
            }
            detailed = !detailed;
        }
        else
        {
            timeBeforeNextTap = doubleTapTime;
        }
    }

    public void OnNavigationStarted(Vector3 position)
    {
        navigationHandStartPosition = position;
        navigationStartPosition = transform.position;
    }

    public void OnNavigationUpdated(Vector3 position)
    {
        Vector3 movement = position - navigationHandStartPosition;
        transform.position = navigationStartPosition + movement * movementSensibility;

        //drawing links
        foreach (Link link in links)
        {
            link.Draw(false);
        }
    }
}
