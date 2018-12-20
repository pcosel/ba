using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class Map : AbstractScatterplot
{
    private float xMax, yMax, xMin, yMin;

    public override void Start()
    {
        xAxis = "Longitude";
        yAxis = "Latitude";
        base.Start();
    }

    public override void SetMaxMin(List<Dictionary<string, string>> data)
    {
        //setting max and min
        foreach (Dictionary<string, string> line in data)
        {
            string strx = Regex.Replace(line[xAxis], @"\s+", "");
            string stry = Regex.Replace(line[yAxis], @"\s+", "");

            if (!strx.Equals("") && !stry.Equals(""))
            {
                float x, y;

                if (float.TryParse(strx, out x) && float.TryParse(stry, out y))
                {
                    xMax = xMax == 0 ? x : (xMax < x ? x : xMax);
                    xMin = xMin == 0 ? x : (xMin > x ? x : xMin);
                    yMax = yMax == 0 ? y : (yMax < y ? y : yMax);
                    yMin = yMin == 0 ? y : (yMin > y ? y : yMin);
                }
            }
        }
        print("xMax: " + xMax + " | yMax: " + yMax + "  xMin: " + xMin + " | yMin: " + yMin);
    }

    public override void CreateDots(List<Dictionary<string, string>> data)
    {
        base.CreateDots(data);

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

                //configure dot
                dot.GetComponent<Dot>().x = x.ToString();
                dot.GetComponent<Dot>().y = y.ToString();
                dot.GetComponent<Dot>().id = id;
                dot.GetComponent<Dot>().values = line;

                if (!dot.GetComponent<Dot>().values.ContainsKey("id"))
                {
                    dot.GetComponent<Dot>().values.Add("id", id.ToString());
                }

                x = Mathf.Abs((x - xMin) / (xMax - xMin)) * 300;
                y = Mathf.Abs((y - yMin) / (yMax - yMin)) * 300;

                dot.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
                dot.GetComponent<RectTransform>().localScale = new Vector2(dotSize, dotSize);
                dot.GetComponent<RectTransform>().localRotation = Quaternion.identity;
                dot.GetComponent<Image>().color = dotColor;

                dots[id] = dot;
            }
            id++;
        }
    }
}
