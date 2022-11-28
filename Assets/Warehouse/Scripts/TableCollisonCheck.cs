using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class TableCollisonCheck : MonoBehaviour
{
    //rename class
    private GameObject obj;
    public int packageNumber;


   
    

    void OnTriggerEnter(Collider other)
      {
        // er ikke i brug pga vi spawner på faste plader, kan altid addes igen
        /*
        if (other.gameObject.tag != "Agent" && other.gameObject.tag != "Ground")
        {
            Debug.Log(other.tag);
            Debug.Log("Collison");
            transform.position = new Vector3(Random.Range(0f, 100f), 0, Random.Range(0f, 100f));
        }
        */
      }

}


