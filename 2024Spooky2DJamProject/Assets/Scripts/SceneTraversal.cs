using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTraversal : MonoBehaviour
{
    void Start() {
        Screen.SetResolution(1080, 1080, true);
    }

    public void SlideEnd() {
        var player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.SlideEnd();
    }
}
