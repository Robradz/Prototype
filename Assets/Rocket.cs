using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip explosion;
    [SerializeField] AudioClip success;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem explosionParticles;
    [SerializeField] ParticleSystem successParticles;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    bool collisionOn = true;
    bool thrusting = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.Debug.isDebugBuild)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                collisionOn = !collisionOn;
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                LoadNextLevel();
            }
        }
        if (state == State.Alive)
        {
            ThrustInput();
            RotateInput();
        }
    }

    private void ThrustInput()
    {

        float thrustThisFrame = mainThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
        {
            print(state);
            if (thrusting == false) 
            {
                mainEngineParticles.Play();
                thrusting = true;
            }
            ApplyThrust(thrustThisFrame);
        }
        else
        {
            audioSource.Stop();
            if (thrusting == true)
            {
                mainEngineParticles.Stop();
                thrusting = false;
            }
        }
    }

    private void ApplyThrust(float thrustThisFrame)
    {
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        //mainEngineParticles.Play();
    }

    private void RotateInput()
    {
        rigidBody.freezeRotation = true; // take control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false; // grant physics control of rotation
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisionOn == false) { return; } // Ignore collisions when dead

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break; // Do nothing
            case "Finish":
                successSequence();
                break;
            default:
                deathSequence();
                break;
        }
    }

    private void successSequence()
    {
        successParticles.Play();
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        Invoke("LoadNextLevel", levelLoadDelay); // parameterize time
    }

    private void deathSequence()
    {
        explosionParticles.Play();
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(explosion);
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
        state = State.Alive;
        audioSource.Stop();
        explosionParticles.Stop();
    }

    private void LoadNextLevel()
    {
        int sceneNum = SceneManager.GetActiveScene().buildIndex;
        print(sceneNum);
        print(SceneManager.sceneCountInBuildSettings);
        int nextScene = sceneNum >= SceneManager.sceneCountInBuildSettings - 1 ? 0 : sceneNum + 1;
        print(nextScene);
        SceneManager.LoadScene(nextScene);
        state = State.Alive;
        audioSource.Stop();
        successParticles.Stop();
    }
}
