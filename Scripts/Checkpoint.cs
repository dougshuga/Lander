using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    private bool triggered = false;
    private Game game;

    // Start is called before the first frame update
    void Start()
    {
        game = FindObjectOfType<Game>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.GetComponent<Rocket>())
        {
            triggered = true;
            game.SetCheckpoint(transform);
        }
    }
}
