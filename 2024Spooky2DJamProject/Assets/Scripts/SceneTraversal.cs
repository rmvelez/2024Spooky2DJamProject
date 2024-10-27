using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTraversal : MonoBehaviour
{
    public void SlideEnd() {
        var player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.SlideEnd();
    }
}
