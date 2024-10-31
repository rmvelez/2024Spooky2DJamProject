using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackScreen : MonoBehaviour
{
    private Animator animator;
    private Player player;
    void Start() {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    public void Reset() {
        animator.SetTrigger("FadeOut");
    }

    // public void FadeOutOver() {
    //     animator.SetTrigger("FadeIn");
    //     player.ResetPosition();
    // }

    public void FadeInOver() {
        player.ResetEnd();
    }
}
