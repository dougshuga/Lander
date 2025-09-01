using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    Rocket rocket;
    AudioSource cameraAudioSource;
    [SerializeField] AudioClip victoryAudioClip;
    private int levelNumber;
    private bool loadingNextLevel = false;
    public List<Transform> checkpoints = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        rocket = FindObjectOfType<Rocket>();
        levelNumber = SceneManager.GetActiveScene().buildIndex;
        cameraAudioSource = FindObjectOfType<Camera>().GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            LoadNextLevel();
        }

        if (rocket.landedOnFinish)
        {
            if (!loadingNextLevel)
            {
                Invoke("LoadNextLevel", 5);
                StartCoroutine(rocket.Celebrate());
                cameraAudioSource.PlayOneShot(victoryAudioClip);
                loadingNextLevel = true;
            }
        }
        else if (!rocket)
        {
            ReloadLevel();
        }
    }

    private void ReloadLevel()
    {
        SceneManager.LoadScene(levelNumber);
    }

    private void LoadNextLevel()
    {
        if (rocket && !rocket.exploded)
        {
            SceneManager.LoadScene(levelNumber + 1);
        }
        else
        {
            ReloadLevel();
        }
    }

    public void SetCheckpoint(Transform transform)
    {
        checkpoints.Add(transform);
    }
}
