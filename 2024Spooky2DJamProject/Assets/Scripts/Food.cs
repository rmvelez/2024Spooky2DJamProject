using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private bool alive;
    public int value = 2;
    void Start() {
        alive = true;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (alive) {
            gameObject.SetActive(false);
            alive = false;
            var player = GameObject.FindWithTag("Player").GetComponent<Player>();
            player.hunger += value;
        }
    }
}
