using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{

    private CharacterController _contoller;
    // Start is called before the first frame update
    void Start()
    {
        _contoller = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
    }
    void PlayerInput()
    {
        if (Input.GetKeyDown("w"))
        {
            _contoller.Move(transform.forward);
            Debug.Log("test");
        }
    }
}
