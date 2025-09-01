using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PyramidEntrance : MonoBehaviour
{
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] float secondsToClose = 3;
    [SerializeField] Light directionalLight;
    private bool moving = false;
    private bool doorOpen = true;
    private Rocket rocket;

    // Start is called before the first frame update
    void Start()
    {
        rocket = FindObjectOfType<Rocket>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            MoveDoors();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (doorOpen)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(ToggleDoor());
                doorOpen = false;
            }
        }
    }

    private IEnumerator ToggleDoor()
    {
        moving = true;
        StartCoroutine(TransformLevel());
        yield return new WaitForSeconds(secondsToClose);
        moving = false;
    }

    private void MoveDoors()
    {
        topDoor.transform.Translate(0, 0, -6 * (Time.deltaTime / secondsToClose));
        bottomDoor.transform.Translate(0, 0, 6 * (Time.deltaTime / secondsToClose));
    }

    private IEnumerator TransformLevel()
    {
        directionalLight.enabled = false;
        RenderSettings.ambientIntensity = 0;
        yield return new WaitForSeconds(2);
        rocket.Glow();
    }
}
