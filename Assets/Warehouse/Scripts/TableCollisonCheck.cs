using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCollisonCheck : MonoBehaviour
{
    private GameObject obj;
    
       void OnTriggerEnter(Collider other)
      {

          if (other.gameObject.tag == "Table" || other.gameObject.tag == "TablePackage")
          {
              Debug.Log("Collision");
              transform.position = new Vector3(Random.Range(0f, 100f), 0, Random.Range(0f, 100f));
          }

      }
   
   
}