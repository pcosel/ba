using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class BarChart : AbstractGraph
{ 
    public GameObject barPrefab;
    public float barSize = 5;
    public Color barColor;
    public string value;

    private int max = 0;
    private Dictionary<string, int> values = new Dictionary<string, int> ();
    private Dropdown valueDropdown;
    private List<GameObject> bars = new List<GameObject>();

    public override void Start()
    {
        valueDropdown = transform.Find("Dropdown/ValueDropdown").GetComponent<Dropdown>();
        base.Start();
    }

    private void Update()
    {
        print(bars.Count);
    }

    public override void Draw(List<Dictionary<string, string>> data)
    {
        drawn = false;

        value = valueDropdown.options[valueDropdown.value].text;
        values = new Dictionary<string, int>();
        max = 0;

        //accumulate values
        foreach (Dictionary<string, string> line in data)
        {
            string str = Regex.Replace(line[value], @"\s+", "");
            str = str.ToLower();
            str = str.Equals("") ? "N/A" : str;

            if (!values.ContainsKey(str))
            {
                values.Add(str, 1);
                max = max == 0 ? 1 : max;
            }
            else
            {
                values[str]++;
                max = max < values[str] ? values[str] : max;
            }
        }

        dots = new GameObject[values.Count];

        //creating bars
        int i = 0;
        foreach (KeyValuePair<string, int> entry in values)
        {
            GameObject bar = Instantiate(barPrefab) as GameObject;
            bar.transform.SetParent(panel.transform.Find("Bars"));

            //position
            float width = 300 / (2 * values.Count + 1);
            float x = 2 * width * (i+1);
            bar.GetComponent<RectTransform>().localPosition = new Vector3(x, 0, 0);
            bar.GetComponent<RectTransform>().localScale = Vector3.one;

            //labels
            bar.transform.Find("Label").GetComponent<Text>().text = entry.Key;
            bar.transform.Find("Count").GetComponent<Text>().text = "" + entry.Value;

            //size
            bar.transform.Find("Image").GetComponent<Image>().fillAmount = (float)entry.Value / max;
            float y = (float)entry.Value / max * 280;
            bar.transform.Find("Count").GetComponent<RectTransform>().localPosition = new Vector3(0, y, 0);
            bar.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(width, 280);
            bar.GetComponent<RectTransform>().localRotation = Quaternion.identity;

            //color
            bar.transform.Find("Image").GetComponent<Image>().color = RandomColor();

            //configure dot
            bar.transform.Find("Image").GetComponent<Dot>().x = entry.Key;
            bar.transform.Find("Image").GetComponent<Dot>().y = entry.Value.ToString();
            bar.transform.Find("Image").GetComponent<Dot>().id = i;
            bar.transform.Find("Image").GetComponent<Dot>().values.Add(value, entry.Key);

            bars.Add(bar);
            dots[i] = bar.transform.Find("Image").gameObject;

            i++;
        }

        drawn = true;
    }

    //function to be called by dropdowns
    public void Draw()
    {
        //remove old bars
        foreach (GameObject bar in bars)
        {
            print("blub");
            GameObject.Destroy(bar);
        }

        Draw(GameObject.Find("CSV").GetComponent<CSV>().data);

        //draw links
        foreach (Link link in links)
        {
            link.Draw(false);
        }
    }

    private Color RandomColor()
    {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);
        return new Color(r, g, b);
    }
}
