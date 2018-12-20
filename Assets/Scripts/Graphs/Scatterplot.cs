using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class Scatterplot : MonoBehaviour {

    public GameObject dotPrefab;
    public GameObject xMarkPrefab;
    public GameObject yMarkPrefab;
    public float dotSize = 5;
    public float dotOpacity;
    public Color dotColor;
    public int maxMarkCount = 10;
    public string xAxis;
    public string yAxis;
    public GameObject[] dots;
    public bool drawn = false;

    private List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
    private GameObject panel;
    private float xMax, yMax, xMin, yMin;
    private DateTime dateMax, dateMin;


    void Start () {
        //get data from csv
        data = GameObject.Find("CSV").GetComponent<CSV>().data;

        dots = new GameObject[data.Count];

        panel = transform.Find("Graph").gameObject;
        transform.Find("xAxis/xAxisLabel").GetComponent<Text>().text = xAxis;
        transform.Find("yAxis/yAxisLabel").GetComponent<Text>().text = yAxis;

        Draw();
    }

    private void Draw()
    {
        drawn = false;

       
        //creating dots
        int id = 0;
        foreach (Dictionary<string, string> line in data)
        {
            string strx = Regex.Replace(line[xAxis], @"\s+", "");
            string stry = Regex.Replace(line[yAxis], @"\s+", "");

            float x, y;

            if (float.TryParse(strx, out x) && float.TryParse(stry, out y))
            {

                GameObject dot = Instantiate(dotPrefab) as GameObject;
                dot.transform.SetParent(panel.transform);

                dot.GetComponent<Dot>().x = x.ToString();
                dot.GetComponent<Dot>().y = y.ToString();
                dot.GetComponent<Dot>().id = id;

                x = Mathf.Abs((x - xMin) / (xMax - xMin)) * 300;
                y = Mathf.Abs((y - yMin) / (yMax - yMin)) * 300;

                dot.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
                dot.GetComponent<RectTransform>().localScale = new Vector2(dotSize, dotSize);
                dot.GetComponent<RectTransform>().localRotation = Quaternion.identity;
                dot.GetComponent<Image>().color = dotColor;

                dots[id] = dot;
            }


            DateTime date;
            DateTime dateMinWithoutTime = new DateTime(dateMin.Year, dateMin.Month, dateMin.Day, 0, 0, 0);
            DateTime dateMaxWithoutTime = new DateTime(dateMax.Year, dateMax.Month, dateMax.Day, 0, 0, 0);

            if (DateTime.TryParse(strx + " " + stry, out date) && date.Year >= 2017)
            {
                GameObject dot = Instantiate(dotPrefab) as GameObject;
                dot.transform.SetParent(panel.transform);

                dot.GetComponent<Dot>().x = date.Month + "/" + date.Day + "/" + date.Year;
                dot.GetComponent<Dot>().y = date.Hour + ":" + date.Minute;
                dot.GetComponent<Dot>().id = id;

                DateTime dateWithoutTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);

                x = Mathf.Abs((float)(dateWithoutTime - dateMinWithoutTime).TotalDays / (float)(dateMaxWithoutTime - dateMinWithoutTime).TotalDays) * 300;

                TimeSpan time = new TimeSpan(date.Hour, date.Minute, 0);

                y = Mathf.Abs((float)time.TotalMinutes / 1440) * 300;

                dot.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
                dot.GetComponent<RectTransform>().localScale = new Vector2(dotSize, dotSize);
                dot.GetComponent<RectTransform>().localRotation = Quaternion.identity;
                dot.GetComponent<Image>().color = dotColor;

                dots[id] = dot;
            }
            id++;
        }

        /*
        //creating marks for x axis
        int xMarkCount = xMax < maxMarkCount ? (int)xMax : maxMarkCount;
        for (int i = 0; i < xMarkCount; i++)
        {
            GameObject mark = Instantiate(xMarkPrefab) as GameObject;
            int value = Mathf.RoundToInt(xMax / xMarkCount * i);
            mark.transform.Find("Label").GetComponent<Text>().text = "" + value;

            mark.transform.SetParent(panel.transform.Find("xAxis"));

            float xPosition = 300 / xMarkCount * i;
            mark.GetComponent<RectTransform>().localPosition = new Vector3(xPosition, 0, 0);
        }

        //creating marks for y axis
        int yMarkCount = yMax < maxMarkCount ? (int)yMax : maxMarkCount;
        for (int i = 0; i < yMarkCount; i++)
        {
            GameObject mark = Instantiate(yMarkPrefab) as GameObject;
            int value = Mathf.RoundToInt(yMax / yMarkCount * i);
            mark.transform.Find("Label").GetComponent<Text>().text = "" + value;

            mark.transform.SetParent(panel.transform.Find("yAxis"));

            float yPosition = 300 / yMarkCount * i;
            mark.GetComponent<RectTransform>().localPosition = new Vector3(0, yPosition, 0);
        }
        */

        drawn = true;
    }
}
