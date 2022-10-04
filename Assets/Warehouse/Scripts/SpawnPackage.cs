using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class SpawnPackage : TableCollisonCheck
{

    [SerializeField] GameObject myPrefab;
    public GameObject tableSpawned;
    public Material[] randomMaterials;
    public int packageNumber;

    void Start()
    {

        tableSpawned = Instantiate(myPrefab, new Vector3(Random.Range(0f, 100f), 0, Random.Range(0f, 100f)), Quaternion.identity);
        //Instantiate(myPrefab, new Vector3(0,0,10), Quaternion.identity);

        //Need to be placed under Resources/ "Will try to find a workaround
        randomMaterials = Resources.LoadAll("Materials", typeof(Material)).Cast<Material>().ToArray();
        int randomNumberMaterial = packageNumber = Random.Range(0, randomMaterials.Length);

        //Assign random color to object will later add so each is a different package
        ChangeMaterial(randomMaterials[randomNumberMaterial],gameObject);
        ChangeMaterial(randomMaterials[randomNumberMaterial],tableSpawned); 


    }

    void ItemDelivered()
    {
        Destroy(tableSpawned);
        Destroy(gameObject);
        
    }
     void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            ItemDelivered();
        }
    }


    void ChangeMaterial(Material newMat,GameObject obj)
    {
        Renderer[] children;
        children = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in children)
        {
            var mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.materials.Length; j++)
            {
                mats[j] = newMat;
            }
            rend.materials = mats;
        }
    }

}