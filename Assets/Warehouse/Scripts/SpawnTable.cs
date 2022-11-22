using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTable : MonoBehaviour
{
    private EnvironmentSettings m_EnvironmentSettings;
    [SerializeField] GameObject table;

    public List<GameObject> tables = new List<GameObject>();
    public List<GameObject> ports = new List<GameObject>();
    public GameObject port;
    public int numberOfTables;
    public List<GameObject> copiedList;
    public void SpawnTables()
    {
       // ports.Clear();
        //copiedList.Clear();
        //Get all ports
        for (int i = 0; i < port.transform.childCount; i++)
        { 
            ports.Add(port.transform.GetChild(i).gameObject);  
        }

        //copy list til spawnPackage
        copiedList = new List<GameObject>(ports);
        
        m_EnvironmentSettings = FindObjectOfType<EnvironmentSettings>();
        int numberOfTables = m_EnvironmentSettings.numberOfTables > ports.Count ? ports.Count: m_EnvironmentSettings.numberOfTables;

        for(int i = 0; i < numberOfTables; i++)
        {
            //instantiate table at port
            int randomNunber = Random.Range(0, ports.Count);
            tables.Add(Instantiate(table, ports[randomNunber].transform.position+ new Vector3(-15,0,15), Quaternion.Euler(0,90f,0),gameObject.transform));
            ports.RemoveAt(randomNunber);
        
            //tables.Add(Instantiate(table, transform.localPosition += new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f)), Quaternion.identity));           
        }
    }
}
