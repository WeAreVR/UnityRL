using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomPackage : MonoBehaviour
{
    // Start is called before the first frame update
    public Material[] randomMaterials;

    void Start()
    {
        //Need to be placed under Resources/ "Will try to find a workaround
        randomMaterials = Resources.LoadAll("Materials", typeof(Material)).Cast<Material>().ToArray();

        //Assign random color to object will later add so each is a different package
        ChangeMaterial(randomMaterials[Random.Range(0, randomMaterials.Length)]);
        
    }

    void ChangeMaterial(Material newMat)
    {
        Renderer[] children;
        children = GetComponentsInChildren<Renderer>();
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
