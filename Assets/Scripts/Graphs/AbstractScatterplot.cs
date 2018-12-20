using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractScatterplot : AbstractGraph {
    [HideInInspector]
    public string xAxis;
    [HideInInspector]
    public string yAxis;
    public float dotSize = 5;
    public Color dotColor;
    public GameObject dotPrefab;


    public virtual void SetMaxMin(List<Dictionary<string, string>> data)
    {
    }

    public virtual void CreateDots(List<Dictionary<string, string>> data)
    {
        //delete old dots
        foreach (GameObject dot in dots)
        {
            GameObject.Destroy(dot);
        }

        dots = new GameObject[data.Count];
    }

    public override void Draw(List<Dictionary<string, string>> data)
    {
        drawn = false;
        SetMaxMin(data);
        CreateDots(data);
        drawn = true;
    }
}
