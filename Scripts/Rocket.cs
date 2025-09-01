using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private Rigidbody myRigidBody;
    private MeshRenderer myMeshRender;
    private Material myMaterial;
    private FinishPad finishPad;
    private Game game;

    [SerializeField] public bool invulnerable = false;
    [SerializeField] float thrustFactor = 1000;
    [SerializeField] float rotationFactor = 100;
    [SerializeField] CinemachineVirtualCamera zoomedInCamera;
    [SerializeField] CinemachineVirtualCamera zoomedOutCamera;
    [SerializeField] Vector3 initialVelocity = new Vector3(0, 0, 0);

    private bool flying = true;
    private bool firingThrusters = false;
    public bool exploded = false;
    public bool landedOnFinish;

    [SerializeField] ParticleSystem thrustParticles;
    [SerializeField] ParticleSystem explosionParticles;
    [SerializeField] ParticleSystem winParticles;

    [SerializeField] AudioSource thrusterAudio;
    [SerializeField] AudioSource thudAudio;
    [SerializeField] AudioSource explosionAudio;

    [SerializeField] public GameObject pointLight;

    // Start is called before the first frame update
    void Start()
    {
        landedOnFinish = false;
        myRigidBody = GetComponent<Rigidbody>();
        myRigidBody.velocity = initialVelocity;
        myMeshRender = GetComponent<MeshRenderer>();
        myMaterial = GetComponent<Renderer>().material;
        game = FindObjectOfType<Game>();

        pointLight.GetComponent<Light>().enabled = false;

        thrustParticles.Stop();
        explosionParticles.gameObject.SetActive(false);
        winParticles.gameObject.SetActive(false);

        finishPad = FindObjectOfType<FinishPad>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("velocity magnitude: " + myRigidBody.velocity.magnitude);
        if (myRigidBody.velocity.magnitude > 70)
        {
            StartCoroutine(Explode());
            return;
        }

        DetermineCamera();

        if (!exploded)
        {
            ProcessThrust();
            ProcessRotation();
        }
    }

    public void Glow()
    {
        pointLight.GetComponent<Light>().enabled = true;
        myMaterial.SetColor(
            "_EmissionColor",
            new Color(1, 1, 0, 1)
        );
    }

    private void ProcessThrust()
    {
        if (!landedOnFinish)
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                myRigidBody.AddRelativeForce(Vector3.up * thrustFactor * Time.deltaTime);
                if (!firingThrusters)
                {
                    StartCoroutine(FireThrusters());
                }
            }
        }
    }

    private IEnumerator FireThrusters()
    {
        firingThrusters = true;
        thrustParticles.Play();
        while (Input.GetAxis("Vertical") > 0)
        {
            if (!thrusterAudio.isPlaying)
            {
                thrusterAudio.Play();
            }
            yield return new WaitForSeconds(0.1f);
        }
        firingThrusters = false;
        thrusterAudio.Stop();
        thrustParticles.Stop();
    }


    private void ProcessRotation()
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            if (flying)
            {
                // addresses a bug where collisions with the world continue to affect you.
                myRigidBody.angularVelocity = Vector3.zero;

                transform.Rotate(0, 0, -Input.GetAxis("Horizontal") * rotationFactor * Time.deltaTime);
            }
        }
    }

    private void DetermineCamera()
    {
        if (flying)
        {
            zoomedInCamera.Priority = 0;
            zoomedOutCamera.Priority = 1;
        }
        else
        {
            zoomedInCamera.Priority = 1;
            zoomedOutCamera.Priority = 0;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        flying = true;
        landedOnFinish = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.GetComponent<Destructor>())
        {
            flying = false;
            StartCoroutine(CheckLanding(collision));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            thudAudio.Play();
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Trap")
        {
            StartCoroutine(Explode());
        }
    }

    private IEnumerator CheckLanding(Collision collision)
    {
        var xOffset = transform.position.x - finishPad.transform.position.x;
        bool touchingFinish = collision.gameObject.tag == "Finish";

        if (touchingFinish && finishPad.sticky)
        {
            myRigidBody.freezeRotation = true;
        }

        // this is an awkward hack around isTouching not existing for Unity3D.
        var framesToWait = Math.Round(1.5f / Time.deltaTime);
        for (int i = 0; i < framesToWait; i++)
        {
            if (flying)
            {
                myRigidBody.freezeRotation = false;
                yield break;
            }
            else
            {
                // allow FinishPad to carry the rocket along.
                if (touchingFinish)
                {
                    transform.position = new Vector3(
                        finishPad.transform.position.x + xOffset,
                        transform.position.y,
                        transform.position.z
                    );
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

        // Don't allow victory until rocket is stable.
        if (touchingFinish &&
            myRigidBody.velocity.magnitude < 0.1f &&
            myRigidBody.angularVelocity.z < 0.05f &&
            Math.Abs(transform.rotation.z) < 0.04f
        )
        {
            landedOnFinish = true;
        }
    }

    public IEnumerator Explode()
    {
        if (exploded)
        {
            yield break;
        }

        if (!invulnerable)
        {
            exploded = true;
            myMeshRender.enabled = false;
            myRigidBody.useGravity = false;
            myRigidBody.freezeRotation = true;

            // required because particle systems on an object affect each other.
            thrustParticles.gameObject.SetActive(false);
            winParticles.gameObject.SetActive(false);
            explosionParticles.gameObject.SetActive(true);

            explosionParticles.Play();
            explosionAudio.Play();
            yield return new WaitForSeconds(4);

            if (game.checkpoints.Count > 0)
            {
                myRigidBody.velocity = Vector3.zero;
                Vector3 lastPosition = game.checkpoints.Last<Transform>().position;
                transform.position =
                    new Vector3(
                        lastPosition.x,
                        lastPosition.y,
                        0
                    );

                exploded = false;
                thrustParticles.gameObject.SetActive(true);
                winParticles.gameObject.SetActive(false);
                explosionParticles.Stop();
                explosionParticles.gameObject.SetActive(false);

                transform.rotation = new Quaternion(0, 0, 0, 0);
                myMeshRender.enabled = true;
                myRigidBody.useGravity = true;
                myRigidBody.freezeRotation = false;
            }

            else
            {
                Destroy(gameObject);
            }
        }
    }

    public IEnumerator Celebrate()
    {
        // don't celebrate if you've crashed into the finish pad.
        yield return new WaitForSeconds(.25f); 
        if (!exploded)
        {
            thrustParticles.gameObject.SetActive(false);
            winParticles.gameObject.SetActive(true);
            winParticles.transform.rotation = finishPad.transform.rotation;
            winParticles.Play();
        }
        yield break;
    }
}
