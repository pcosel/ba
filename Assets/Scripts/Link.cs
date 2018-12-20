using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Link : MonoBehaviour {

    public GameObject linePrefab;
    public AbstractGraph graph1, graph2;
    public List<KeyValuePair<int, int>> indices = new List<KeyValuePair<int, int>>();
    public float startWidth = 4, endWidth = 4;
    public int startIndex, endIndex;

    private bool drawn = false;

    void Start()
    {
        graph1.links.Add(this);
        graph2.links.Add(this);
    }

    void Update () {
        //initial drawing of lines when graphs finished drawing
        if(graph1.drawn && graph2.drawn && !drawn)
        {
            if (startIndex >= 0 && endIndex < graph2.dots.Length)
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    GameObject dot = graph2.dots[i];
                    if (dot != null)
                    {
                        Filter(dot.GetComponent<Dot>(), graph1);
                    }
                }

                Draw(true);
            }
            else
            {
                print("StartIndex and/or EndIndex not in bounds.");
            }

        }
	}

    public void Draw(bool instantiateLines)
    {
        foreach (KeyValuePair<int, int> entry in indices)
        {
            LineRenderer line = new LineRenderer();
            if (instantiateLines)
            {
                //delete old line
                if (transform.Find("Line" + entry.Key + entry.Value) != null)
                {
                    GameObject.Destroy(transform.Find("Line" + entry.Key + entry.Value).gameObject);
                }
                //instantiate line
                line = (Instantiate(linePrefab) as GameObject).GetComponent<LineRenderer>();
                line.transform.parent = this.transform;
                line.gameObject.name = "Line" + entry.Key + "-" + entry.Value;
            } else
            {
                line = transform.Find("Line" + entry.Key + "-" + entry.Value).GetComponent<LineRenderer>();
            }

            //set start and end position
            RectTransform rt = graph1.dots[entry.Key].GetComponent<RectTransform>();
            Vector3 pos1 = graph1.dots[entry.Key].transform.position;
            //Vector3 pos1 = new Vector3(rt.position.x, rt.position.y, rt.position.z);

            rt = graph2.dots[entry.Value].GetComponent<RectTransform>();
            Vector3 pos2 = graph2.dots[entry.Value].transform.position;
            //Vector3 pos2 = new Vector3(rt.position.x, rt.position.y, rt.position.z);

            line.SetPositions(new Vector3[] { pos1, pos2 });

            //set start and end color
            Color dotColor = graph2.dots[entry.Value].GetComponent<Image> ().color;
            line.startColor = new Color(dotColor.r, dotColor.g, dotColor.b);
            dotColor = graph1.dots[entry.Key].GetComponent<Image>().color;
            line.endColor = new Color(dotColor.r, dotColor.g, dotColor.b);

            //set start and end width
            line.startWidth = startWidth / 1000;
            line.endWidth = endWidth / 1000;
        }
        drawn = true;
    }

    public void Filter(Dot dot, AbstractGraph graphTarget)
    {
        string commonValue = CommonValue(dot, graphTarget);
        print("Common value of " + graph1.name + " and " + graph2.name + ": " + commonValue);

        foreach (GameObject dotT in graphTarget.dots)
        {
            if (dotT != null && dotT.GetComponent<Dot>().values.ContainsKey(commonValue) && dot.values.ContainsKey(commonValue))
            {
                //formatting value strings
                string dotValue = dot.values[commonValue].ToLower().Replace(" ", string.Empty);
                string dotTValue = dotT.GetComponent<Dot>().values[commonValue].ToLower().Replace(" ", string.Empty);

                if(dotValue == dotTValue)
                {
                    indices.Add(new KeyValuePair<int, int>(dotT.GetComponent<Dot>().id, dot.id));
                }
            }
        }
    }

    private string CommonValue(Dot dot, AbstractGraph graph)
    {
        foreach (GameObject dotT in graph.dots)
        {
            if (dotT != null && dotT.GetComponent<Dot>().values.ContainsKey("id") && dot.values.ContainsKey("id"))
            {
                return "id";
            }
            else
            {
                foreach (KeyValuePair<string, string> entry in dot.values)
                {
                    if(dotT.GetComponent<Dot>().values.ContainsKey(entry.Key))
                    {
                        return entry.Key;
                    }
                }
            }
        }

        return "";
    }
}
