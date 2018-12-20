using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour {

    public bool moving;
    public float distanceToCamera;
    public List<AbstractGraph> graphs = new List<AbstractGraph>();

    public void OnSelect()
    {
        print("Select " + this.name);
        if (!moving)
        {
            distanceToCamera = Vector3.Distance(transform.position, GetComponent<Camera>().transform.position);
        }
        moving = !moving;
    }
}
