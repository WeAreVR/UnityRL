using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TableCollisonCheck : MonoBehaviour
{
    //rename class
    private GameObject obj;
    public int packageNumber;
    public Material[] randomMaterials;
    public GameObject[] shelf;

   


    void OnTriggerEnter(Collider other)
      {

          if (other.gameObject.tag != "Agent" )
          {
              transform.position = new Vector3(Random.Range(0f, 100f), 0, Random.Range(0f, 100f));
          }

      }
   
   
}
