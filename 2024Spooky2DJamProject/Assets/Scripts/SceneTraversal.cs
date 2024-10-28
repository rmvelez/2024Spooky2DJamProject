using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTraversal : MonoBehaviour
{
    void Start() {
        Screen.SetResolution(640, 640, false);
        //Camera.main.aspect = 1;
    }

    public void SlideEnd() {
        var player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.SlideEnd();
    }
}
