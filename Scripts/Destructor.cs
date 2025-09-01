using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructor : MonoBehaviour
{
    private Rocket rocket;
    // Start is called before the first frame update
    void Start()
    {
        rocket = transform.parent.gameObject.GetComponent<Rocket>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player" && other.gameObject.tag != "Checkpoint")
        {
            if (!rocket.invulnerable)
            {
                StartCoroutine(rocket.Explode());
            }
        }
    }
}
