using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private bool alive;
    public int value = 2;
    private AudioSource audioPlayer;
    void Start() {
        alive = true;
        audioPlayer = GameObject.FindWithTag("SFX").GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (alive) {
            AudioClip clip;
            if (gameObject.name != "Water") {
                clip = (AudioClip)Resources.Load("Audio/SFX/Food Eat");
            } else {
                clip = (AudioClip)Resources.Load("Audio/SFX/Drink SFX");
            }
            audioPlayer.PlayOneShot(clip);
            gameObject.SetActive(false);
            alive = false;
            var player = GameObject.FindWithTag("Player").GetComponent<Player>();
            player.hunger += value;
        }
    }
}
