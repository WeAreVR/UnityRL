using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCollisonCheck : MonoBehaviour
{
    public float sphereRadius;

    
       void OnTriggerEnter(Collider other)
      {

          if (other.gameObject.tag == "Table")
          {
              Debug.Log("Collision");
              transform.position = new Vector3(Random.Range(0f, 100f), 0, Random.Range(0f, 100f));
          }
      }
   
   
}
