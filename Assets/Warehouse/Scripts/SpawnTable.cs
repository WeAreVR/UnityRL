using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class SpawnTable : MonoBehaviour
{
    private EnvironmentSettings m_EnvironmentSettings;
    [SerializeField] GameObject table;
    [SerializeField] GameObject package;

    public List<GameObject> tables = new List<GameObject>();
    public List<GameObject> packages = new List<GameObject>();
    public List<GameObject> ports = new List<GameObject>();
    public List<GameObject> rows = new List<GameObject>();
    public GameObject row;
    public Material[] randomMaterials;
    //public List<Material> copyOfMaterial = new List<Material>();
    public Dictionary<Material, int> dict =
            new Dictionary<Material, int>();


    public GameObject port;
    public int numberOfTables;
    public List<GameObject> copiedList;
    private int packageNumber;
    private GameObject tablePrefab;
    private GameObject tablePackage;

    public void Awake()
    {
        //Need to be placed under Resources/ "Will try to find a workaround
         randomMaterials = Resources.LoadAll("Materials", typeof(Material)).Cast<Material>().ToArray();
    }

   

  
    /*public void RandomDeliverySite()
    {

        for(int i = 0; i< randomMaterials.Length;i++)
        {
            dict.Add(randomMaterials[i], i);

        }
        for (int i = 0; i < port.transform.childCount; i++)
        {
            

            ports.Add(port.transform.GetChild(i).gameObject);
            int randomNumberMaterial = packageNumber = Random.Range(0, dict.Count);
            ChangeMaterial(randomMaterials[randomNumberMaterial], port.transform.GetChild(i).gameObject, "Table", packageNumber);
            var removeMat = randomMaterials[randomNumberMaterial];
            dict.Remove(removeMat);
        }

    }
    */
    public void SpawnTables()
    {
        ClearEverything();

        for (int i = 0; i < port.transform.childCount; i++)
        {
            ports.Add(port.transform.GetChild(i).gameObject);
        }
        m_EnvironmentSettings = FindObjectOfType<EnvironmentSettings>();

        for (int i = 0; i < row.transform.childCount; i++)
        {
            rows.Add(row.transform.GetChild(i).gameObject);
        }

        int numberOfTables = m_EnvironmentSettings.numberOfTables > rows.Count ? rows.Count : m_EnvironmentSettings.numberOfTables;

        for (int i = 0; i < numberOfTables; i++)
        {
            int randomNumber = Random.Range(0, rows.Count);
            int packageNumber = Random.Range(0, randomMaterials.Length);

            tablePrefab = rows[randomNumber];
            //tablePrefab.layer =LayerMask.NameToLayer("Package");;
            tablePrefab.layer = 3;
            foreach (Transform child in tablePrefab.transform)
            {
                child.gameObject.layer = 3;
            }

            packages.Add(tablePrefab);
            ChangeMaterial(randomMaterials[packageNumber], tablePrefab, "TablePackage", packageNumber);
            rows.Remove(rows[randomNumber]);

        }
    }

     void ClearEverything()
    {
        rows.Clear();
        ports.Clear();
        for (int i = 0; i < packages.Count; i++)
        {
            RemoveMat(packages[i]);
        }
        packages.Clear();

    }
    public void RemoveMat(GameObject obj)
    {

        var children = obj.GetComponentsInChildren<Renderer>();
        obj.tag = "wall";
        obj.layer = 0;

        foreach (Renderer rend in children)
        {
            rend.tag = "wall";
            rend.gameObject.layer = 0;

            rend.material.color = Color.white;

        }
        
        obj.GetComponent<TableCollisonCheck>().packageNumber = -1;
        obj.tag = "wall";

    }


    void ChangeMaterial(Material newMat, GameObject obj, string tagName, int number)
    {
        Renderer[] children;
        children = obj.GetComponentsInChildren<Renderer>();
        obj.GetComponent<TableCollisonCheck>().packageNumber = number;

        //NOTE: vi slettet +packageNumber i linje 49
        obj.tag = tagName;
        foreach (Renderer rend in children)
        {
            rend.tag = tagName;
            var mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.materials.Length; j++)
            {
                mats[j] = newMat;
            }
            rend.materials = mats;
        }

    }
    /*
    public void SpawnTables()
    {
        ports.Clear();
        
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
            tablePrefab = Instantiate(table, ports[randomNunber].transform.position + new Vector3(-15, 0, 15), Quaternion.Euler(0, 90f, 0), gameObject.transform);
            tables.Add(tablePrefab);
            ports.RemoveAt(randomNunber);
            SpawnPackage(tablePrefab);

            //tables.Add(Instantiate(table, transform.localPosition += new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f)), Quaternion.identity));           
        }
    }
    
    void SpawnPackage(GameObject tablePrefab) {
        //tableSpawned = Instantiate(myPrefab, new Vector3(Random.Range(0f, 100f), 0, Random.Range(0f, 100f)), Quaternion.Euler(0, 90f, 0));
        //add random range til new vector z hvis de skal være lidt forskellige
        //m_SpawnTable = FindObjectOfType<SpawnTable>();
        int randomNunber = Random.Range(0, copiedList.Count);
        tablePackage = Instantiate(package, copiedList[randomNunber].transform.position + new Vector3(-15, 0, 45), Quaternion.Euler(0, 90f, 0), gameObject.transform.root);
        packages.Add(tablePackage);
        copiedList.RemoveAt(randomNunber);


        //Need to be placed under Resources/ "Will try to find a workaround
        var randomMaterials = Resources.LoadAll("Materials", typeof(Material)).Cast<Material>().ToArray();
        int randomNumberMaterial = packageNumber = Random.Range(0, randomMaterials.Length);

        //Assign random color to object will later add so each is a different package

        ChangeMaterial(randomMaterials[randomNumberMaterial], tablePackage, packageNumber, "TablePackage");
        ChangeMaterial(randomMaterials[randomNumberMaterial], tablePrefab, packageNumber, "Table");
        
    }

    void ChangeMaterial(Material newMat, GameObject obj, int number, string tagName)
    {
        Renderer[] children;
        children = obj.GetComponentsInChildren<Renderer>();
        obj.GetComponent<TableCollisonCheck>().packageNumber = number;

        //NOTE: vi slettet +packageNumber i linje 49
        obj.tag = tagName;
        foreach (Renderer rend in children)
        {
            rend.tag = tagName;
            var mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.materials.Length; j++)
            {
                mats[j] = newMat;
            }
            rend.materials = mats;
        }

    }
    */
}
    