using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTable : MonoBehaviour
{
    private EnvironmentSettings m_EnvironmentSettings;
    [SerializeField] GameObject table;
    // Start is called before the first frame update
    void Start()
    {
        m_EnvironmentSettings = FindObjectOfType<EnvironmentSettings>();

        for(int i = 0; i < m_EnvironmentSettings.numberOfTables; i++)
        {
            Instantiate(table, new Vector3(Random.Range(0f, 100f), 0, Random.Range(0f, 100f)), Quaternion.identity);

        }

    }


}