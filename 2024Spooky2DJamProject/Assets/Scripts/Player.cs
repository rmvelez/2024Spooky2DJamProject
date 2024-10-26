using UnityEngine;
public class Player : MonoBehaviour
{
    private Vector2 screenBounds;
    private Animator animator;
    private bool facingRight;
    public int moveSpeed = 6;
    public int hunger;

    void Start() {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        animator = GetComponent<Animator>();
        facingRight = true;
    }

    // Update is called once per frame
    void Update() {
        Vector2 playerPos = transform.position;

        var vertical = Input.GetAxisRaw("Vertical");
        var horizontal = Input.GetAxisRaw("Horizontal");

        Vector2 direction = new(0,0);
        if (horizontal > 0) {
            direction.x = 1;
            if (!facingRight) {
                gameObject.transform.localScale = new Vector3(1,1,1);
                facingRight = true;
            }
        }
        else if (horizontal < 0) {
            direction.x = -1;
            if (facingRight) {
                gameObject.transform.localScale = new Vector3(-1,1,1);
                facingRight = false;
            }
        }
        else if (vertical > 0) {
            direction.y = 1;
        }
        else if (vertical < 0) {
            direction.y = -1;
        }
        setAnimation(direction);

        Vector2 displacement = 0.001f * moveSpeed * direction;
        playerPos += displacement;

        //keep player within screen bounds
        playerPos.x = Mathf.Clamp(playerPos.x, -1 * screenBounds.x, screenBounds.x);
        playerPos.y = Mathf.Clamp(playerPos.y, -1 * screenBounds.y, screenBounds.y);

        transform.position = playerPos;
    }

    void setAnimation(Vector2 direction) {
        animator.SetBool("idle", direction.x == direction.y);
        animator.SetBool("walkUp", direction.y == 1);
        animator.SetBool("walkDown", direction.y == -1);
        animator.SetBool("walkSide", direction.x != 0);
    }
}
