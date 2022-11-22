using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class SpawnPackage : TableCollisonCheck
{

    [SerializeField] GameObject myPrefab;
    public GameObject tableSpawned;
    private SpawnTable m_SpawnTable;

    void Start()
    {

        //tableSpawned = Instantiate(myPrefab, new Vector3(Random.Range(0f, 100f), 0, Random.Range(0f, 100f)), Quaternion.Euler(0, 90f, 0));
        //add random range til new vector z hvis de skal være lidt forskellige
        //m_SpawnTable = FindObjectOfType<SpawnTable>();
        m_SpawnTable = gameObject.transform.root.GetComponent<SpawnTable>();
        int randomNunber = Random.Range(0, m_SpawnTable.copiedList.Count);
        tableSpawned = Instantiate(myPrefab, m_SpawnTable.copiedList[randomNunber].transform.position + new Vector3(-15, 0, 150), Quaternion.Euler(0, 90f, 0),gameObject.transform.root);
        m_SpawnTable.copiedList.RemoveAt(randomNunber);


        //Need to be placed under Resources/ "Will try to find a workaround
        randomMaterials = Resources.LoadAll("Materials", typeof(Material)).Cast<Material>().ToArray();
        int randomNumberMaterial = packageNumber = Random.Range(0, randomMaterials.Length);

        //Assign random color to object will later add so each is a different package
        ChangeMaterial(randomMaterials[randomNumberMaterial], gameObject,packageNumber,"Table");
        ChangeMaterial(randomMaterials[randomNumberMaterial], tableSpawned, packageNumber, "TablePackage");
    }

    public void ItemDelivered(GameObject pairedTable)
    {
        Destroy(gameObject);
        Destroy(pairedTable);

    }


    void ChangeMaterial(Material newMat,GameObject obj,int number,string tagName)
    {
        Renderer[] children;
        obj.GetComponent<TableCollisonCheck>().packageNumber = number;
        children = obj.GetComponentsInChildren<Renderer>();

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

}
