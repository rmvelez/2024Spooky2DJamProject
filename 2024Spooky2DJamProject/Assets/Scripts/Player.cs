using UnityEngine;
public class Player : MonoBehaviour
{
    private Vector2 screenBounds;
    public int moveSpeed = 4;
    public int hunger;

    void Start() {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

    // Update is called once per frame
    void Update() {
        Vector2 playerPos = transform.position;
        //keyboard input
        Vector2 input = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 displacement = 0.001f * moveSpeed * Vector3.Normalize(input);
        playerPos += displacement;

        //keep player within screen bounds
        playerPos.x = Mathf.Clamp(playerPos.x, -1 * screenBounds.x, screenBounds.x);
        playerPos.y = Mathf.Clamp(playerPos.y, -1 * screenBounds.y, screenBounds.y);

        transform.position = playerPos;
    }
}
