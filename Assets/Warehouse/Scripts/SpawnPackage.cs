using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPackage : MonoBehaviour
{

    [SerializeField] GameObject myPrefab;
    public GameObject tableSpawned;
    void Start()
    {
         tableSpawned = Instantiate(myPrefab, new Vector3(Random.Range(0f, 100f),0, Random.Range(0f, 100f)), Quaternion.identity);
        //Instantiate(myPrefab, new Vector3(0,0,10), Quaternion.identity);

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

}
