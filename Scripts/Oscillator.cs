using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    Vector3 startingPosition;
    [SerializeField] Vector3 movementVector;
    [SerializeField] float periodInSeconds = 10f;
    [SerializeField] float cycleOffset = 0;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float cycles = (Time.time / periodInSeconds) + cycleOffset;

        const float tau = Mathf.PI * 2;
        float rawSineValue = Mathf.Sin(tau * cycles);

        Vector3 offset = movementVector * rawSineValue;
        transform.position = startingPosition + offset;
    }
}
