using UnityEngine;

public class Darkness : MonoBehaviour
{
    SpriteRenderer image;
    float currAlpha;
    void Start() {
        image = GetComponent<SpriteRenderer>();
        currAlpha = 0;
        image.color = new Color(0,0,0,0);
    }

    public void Darken() {
        currAlpha += 0.1f;
        image.color = new Color(0,0,0,currAlpha);
    }
}