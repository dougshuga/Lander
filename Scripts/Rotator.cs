using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// rotates the object endlessly
public class Rotator : MonoBehaviour
{
    [SerializeField ]float xSpeed = 0;
    [SerializeField] float ySpeed = 0;
    [SerializeField] float zSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(
            Time.deltaTime * new Vector3(xSpeed, ySpeed, zSpeed)
        );
    }
}
