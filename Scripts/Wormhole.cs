using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Wormhole : MonoBehaviour
{

    Camera mainCamera;
    AudioSource audioSource;
    [SerializeField] AudioClip teleportSound;
    [SerializeField] float delayTime = 3;
    private ParticleSystem myParticleSystem;
    private bool teleporting = false;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        audioSource = mainCamera.GetComponent<AudioSource>();
        myParticleSystem = GetComponent<ParticleSystem>();
        myParticleSystem.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        // wait for song to end, then trigger reload of game.
        if (!teleporting && !audioSource.isPlaying)
        {
            teleporting = true;
            StartCoroutine(Teleport());
        }
    }

    private IEnumerator Teleport()
    {
        audioSource.clip = teleportSound;
        audioSource.Play();
        yield return new WaitForSeconds(delayTime);
        myParticleSystem.Play();
        yield return new WaitForSeconds(teleportSound.length - delayTime);
        SceneManager.LoadScene(0);
    }
}
