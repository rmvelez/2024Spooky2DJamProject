using UnityEngine;
public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    private Animator animator;
    private Animator cameraAnimator;
    private Vector2 screenBounds;
    private string[] directions = {"Up", "Right", "Right", "Down", "Right", "Up", "Up", "Right", "Up", "Up"};
    private int currScene = 0;
    private bool facingRight;
    private bool sliding;
    private string transitionDir;

    public int moveSpeed = 6;
    public int hunger;

    void Start() {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cameraAnimator = Camera.main.GetComponent<Animator>();
        facingRight = true;
    }

    void Update() {
        Vector2 playerPos = transform.position;

        Vector2 direction = new(0,0);
        if (!sliding) {
            //normal movement
            var vertical = Input.GetAxisRaw("Vertical");
            var horizontal = Input.GetAxisRaw("Horizontal");
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
        } else {
            //movement during screen transition
            if (transitionDir == "Up") {
                direction.y = 1;
            } else if (transitionDir == "Down") {
                direction.y = -1;
            } else if (transitionDir == "Right") {
                direction.x = 1;
            } else {
                direction.x = -1;
            }
        }
        SetAnimation(direction);

        //computes displacement per frame
        Vector2 displacement = 0.01f * moveSpeed * direction;
        playerPos += displacement;

        if (!sliding) {
            var camSize = Camera.main.orthographicSize;
            //boundary checks for screen transition
            if (playerPos.x <= screenBounds.x - camSize * 2f) {
                SceneChange("Left");
            } else if (playerPos.x >= screenBounds.x) {
                SceneChange("Right");
            } else if (playerPos.y <= screenBounds.y - camSize * 2f) {
                SceneChange("Down");
            } else if (playerPos.y >= screenBounds.y) {
                SceneChange("Up");
            } else {
                //keep player within screen bounds
                playerPos.x = Mathf.Clamp(playerPos.x, -1 * screenBounds.x, screenBounds.x);
                playerPos.y = Mathf.Clamp(playerPos.y, -1 * screenBounds.y, screenBounds.y);
            }
        }

        rb.MovePosition(playerPos);
    }

    void SetAnimation(Vector2 direction) {
        animator.SetBool("idle", direction.x == direction.y);
        animator.SetBool("walkUp", direction.y == 1);
        animator.SetBool("walkDown", direction.y == -1);
        animator.SetBool("walkSide", direction.x != 0);
    }

    void SceneChange(string dir) {
        sliding = true;
        transitionDir = dir;
        if (dir == directions[currScene]) {
            cameraAnimator.SetTrigger("Slide" + dir);
        } else {
            var blackScreen = GameObject.FindWithTag("BlackScreen").GetComponent<BlackScreen>();
            blackScreen.Reset();
        }
    }

    public void SlideEnd() {
        cameraAnimator.SetTrigger("Idle");
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        sliding = false;
        currScene++;
    }

    public void ResetPosition() {
        var camSize = Camera.main.orthographicSize;
        Vector2 newPos = screenBounds;
        if (transitionDir == "Up") {
            newPos.x -= camSize;
            newPos.y -= camSize * 2f;
        } else if (transitionDir == "Down") {
            newPos.x -= camSize;
        } else if (transitionDir == "Left") {
            newPos.y -= camSize;
        } else {
            newPos.x -= camSize * 2f;
            newPos.y -= camSize;
        }
        transform.position = newPos;
        //sliding = false;
        //transform.position = new(screenBounds.x - camSize, screenBounds.y - camSize);
    }

    public void ResetEnd() {
        sliding = false;
    }
}
