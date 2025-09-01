using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarEffect : MonoBehaviour
{
    ParticleSystem myParticles;

    // Start is called before the first frame update
    void Start()
    {
        myParticles = GetComponent<ParticleSystem>();
        myParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
