using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 20f;
    [SerializeField] ParticleSystem mainEngineParticle;
    [SerializeField] ParticleSystem successParticle;
    [SerializeField] ParticleSystem deathParticle;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    enum State { Alive, Dying, Transcending};

    State state = State.Alive;
    Rigidbody rigidBody;
    AudioSource audioSource;
    int currentScene;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        currentScene = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (state == State.Alive) {
            Thrust();
        }
        Rotate();
	}

    private void Thrust() {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) {
            rigidBody.AddRelativeForce(Vector3.up * mainThrust);
            if (!audioSource.isPlaying) {
                audioSource.PlayOneShot(mainEngine);
            }
            mainEngineParticle.Play();
        } else {
            audioSource.Stop();
            mainEngineParticle.Stop();
        }
    }

    private void Rotate() {
        rigidBody.freezeRotation = true;

        float rotationSpeed = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(Vector3.forward * rotationSpeed);
        } else if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(-Vector3.forward * rotationSpeed);
        }

        rigidBody.freezeRotation = false;
    }

    private void OnCollisionEnter(Collision collision) {
        if (state != State.Alive) { return; }

        switch (collision.gameObject.tag) {
            case "Friendly":
                // Do nothing
                break;
            case "Finish":
                state = State.Transcending;
                audioSource.Stop();
                audioSource.PlayOneShot(success);
                successParticle.Play();
                Invoke("upgradeLevel", 1.5f);
                break;
            default:
                state = State.Dying;
                audioSource.Stop();
                audioSource.PlayOneShot(death);
                deathParticle.Play();
                Invoke("resetLevel", 1f);
                break;
        }
    }

    private void upgradeLevel() {

        currentScene++;
        SceneManager.LoadScene(currentScene);
    }

    private void resetLevel() {
        currentScene = 0;
        SceneManager.LoadScene(currentScene);
    }
}
