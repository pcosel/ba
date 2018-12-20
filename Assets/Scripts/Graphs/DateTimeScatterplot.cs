using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class DateTimeScatterplot : AbstractScatterplot {

    public GameObject xMarkPrefab;

    private DateTime dateMax, dateMin;
    private Dictionary<string, int> months = new Dictionary<string, int>();
    private Dropdown monthMinDropdown, monthMaxDropdown, yearMinDropdown, yearMaxDropdown;
    private List<GameObject> xMarks = new List<GameObject>();
    private GameObject errorText;

    public override void Start()
    {
        xAxis = "CrimeTime";
        yAxis = "CrimeDate";

        //setup months dictionary
        months.Add("Jan.", 1);
        months.Add("Feb.", 2);
        months.Add("Mar.", 3);
        months.Add("Apr.", 4);
        months.Add("May", 5);
        months.Add("Jun.", 6);
        months.Add("Jul.", 7);
        months.Add("Aug.", 8);
        months.Add("Sep.", 9);
        months.Add("Okt.", 10);
        months.Add("Nov.", 11);
        months.Add("Dec.", 12);

        //find dropdowns
        monthMinDropdown = transform.Find("Dropdowns/MonthMinDropdown").GetComponent<Dropdown>();
        monthMaxDropdown = transform.Find("Dropdowns/MonthMaxDropdown").GetComponent<Dropdown>();
        yearMinDropdown = transform.Find("Dropdowns/YearMinDropdown").GetComponent<Dropdown>();
        yearMaxDropdown = transform.Find("Dropdowns/YearMaxDropdown").GetComponent<Dropdown>();

        //find error text
        errorText = transform.Find("ErrorText").gameObject;

        base.Start();
    }

    //function to be called by dropdowns
    public void Draw()
    {
        Draw(GameObject.Find("CSV").GetComponent<CSV>().data);

        //draw links
        foreach (Link link in links)
        {
            link.Draw(false);
        }
    }

    public override void Draw(List<Dictionary<string, string>> data)
    {
        drawn = false;
        SetMaxMin(data);

        //to prevent dateMax beeing smaller than dateMin
        if (dateMax > dateMin)
        {
            //hide error text
            errorText.SetActive(false);
            panel.SetActive(true);

            drawn = false;
            CreateDots(data);
            CreateMarks();
            drawn = true;
        } else
        {
            //display error text
            errorText.SetActive(true);
            panel.SetActive(false);
        }
    }

    public override void SetMaxMin(List<Dictionary<string, string>> data)
    {
        //getting years from dropdowns
        int yearMin = Int32.Parse(yearMinDropdown.options[yearMinDropdown.value].text);
        int yearMax = Int32.Parse(yearMaxDropdown.options[yearMaxDropdown.value].text);

        //getting months from dropdowns
        int monthMin = months[monthMinDropdown.options[monthMinDropdown.value].text];
        int monthMax = months[monthMaxDropdown.options[monthMaxDropdown.value].text];

        //setting max and min
        dateMin = new DateTime(yearMin, monthMin, 1, 0, 0, 0);
        dateMax = new DateTime(yearMax, monthMax, 1, 0, 0, 0);

        print("dateMax: " + dateMax + "  dateMin: " + dateMin);
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

            DateTime date;

            if (DateTime.TryParse(strx + " " + stry, out date) /*&& date < dateMax && date > dateMin*/)
            {
                //instantiating dot
                GameObject dot = Instantiate(dotPrefab) as GameObject;

                //setting parent
                dot.transform.SetParent(panel.transform);

                //configure dot
                dot.GetComponent<Dot>().x = date.Month + "/" + date.Day + "/" + date.Year;
                dot.GetComponent<Dot>().y = date.Hour + ":" + date.Minute;
                dot.GetComponent<Dot>().id = id;
                dot.GetComponent<Dot>().values = line;

                if(!dot.GetComponent<Dot>().values.ContainsKey("id")) {
                    dot.GetComponent<Dot>().values.Add("id", id.ToString());
                }

                DateTime dateWithoutTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);

                x = Mathf.Abs((float)(dateWithoutTime - dateMin).TotalDays / (float)(dateMax - dateMin).TotalDays) * 300;

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
    }

    public void CreateMarks()
    {
        //months between dateMax and dateMin
        int months = ((dateMax.Year - dateMin.Year) * 12) + dateMax.Month - dateMin.Month;

        //months between each mark
        int step = (int) Math.Ceiling((double) months / maxMarkCount);

        //deleting old marks
        foreach (GameObject mark in xMarks)
        {
            Destroy(mark);
        }

        //gernerating marks every step months
        DateTime date = dateMin;
        while(date <= dateMax)
        {
            //instantiating mark
            GameObject mark = Instantiate(xMarkPrefab) as GameObject;

            //setting parent and name
            mark.transform.SetParent(transform.Find("xAxis"), false);
            mark.name = date.Month + "/" + date.Year;

            //setting text
            mark.transform.Find("Label").GetComponent<Text>().text = date.Month + "/" + date.Year;

            //setting position and scale
            float x = Mathf.Abs((float)(date - dateMin).TotalDays / (float)(dateMax - dateMin).TotalDays) * 300;
            mark.GetComponent<RectTransform> ().localPosition = new Vector3(x, 0, 0);
            mark.GetComponent<RectTransform>().localScale = Vector3.one;

            //adding mark to list
            xMarks.Add(mark);

            //icreasing date by step months
            date = date.AddMonths(step);
        }
    }
}
