using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSV : MonoBehaviour {

    public string csvPath;
    public List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();

    void Awake () {
        data = CSVReader.Read(csvPath);
    }
}
