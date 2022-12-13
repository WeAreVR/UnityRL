using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using static UnityEditor.Progress;
using Google.Protobuf.Collections;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;

public class PrintToCsv : MonoBehaviour
{
    public List<int> list = new List<int>();
    public GameObject obj;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            list = obj.GetComponent<AgentMover>().timePerEpoch;
            Debug.Log("print");
            PrintList(list);
        }
    }
    public void PrintList<T>(List<T> list)
    {
        //write generic list to csv file
        string path = Application.dataPath + "/CSV/Items.csv";
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            foreach (var item in list)
            {
                writer.WriteLine(item);
            }
        }
    }


}

