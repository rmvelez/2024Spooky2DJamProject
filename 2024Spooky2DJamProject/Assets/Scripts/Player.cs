using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private Animator cameraAnimator;
    private AudioSource audioPlayer;
    private Slider hungerBar;
    private AudioClip[] clips;
    private Vector2 screenBounds;
    private string[] directions = {"Up", "Right", "Right", "Down", "Right", "Up", "Up", "Right", "Up", "Up"};
    private int currScene = 0;
    private bool facingRight;
    private bool sliding;
    private string transitionDir;

    public int moveSpeed = 6;
    public int hunger = 100;
    public int hungerPerSecond = 2;
    private float hungerTickLength;
    private float hungerTime;
    private float creepyLength;
    private float creepyTime;
    void Start() {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cameraAnimator = Camera.main.GetComponent<Animator>();
        audioPlayer = GameObject.FindWithTag("SFX").GetComponent<AudioSource>();
        hungerBar = GameObject.FindWithTag("HungerBar").GetComponent<Slider>();
        hunger = 100;
        hungerTickLength = 1f / hungerPerSecond;
        hungerTime = 0f;
        clips = new AudioClip[]{
            (AudioClip)Resources.Load("Audio/SFX/Correct Path"),
            (AudioClip)Resources.Load("Audio/SFX/Wrong Path"),
            (AudioClip)Resources.Load("Audio/SFX/Footstep 1"),
            (AudioClip)Resources.Load("Audio/SFX/Footstep 2"),
            (AudioClip)Resources.Load("Audio/SFX/Spooky Noise")
        };
        creepyLength = 30f;
        facingRight = true;
        hunger = 100;
        hungerTickLength = 1f / hungerPerSecond;
        hungerTime = 0f;
    }
    void Update() {
        if (hungerTime >= hungerTickLength) {
            hunger -= 1;
            hungerBar.value -= 0.01f;
            hungerTime = 0f;
            if (hunger == 0) {
                SceneManager.LoadScene("LoseScene");
            }
        } else {
            hungerTime += Time.deltaTime;
        }
        if (creepyTime >= creepyLength) {
            audioPlayer.PlayOneShot(clips[4]);
        } else {
            creepyTime += Time.deltaTime;
        }


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
        if (currScene == 9) {
            SceneManager.LoadScene("WinScene");
        }
        sliding = true;
        transitionDir = dir;
        if (dir == directions[currScene]) {
            audioPlayer.PlayOneShot(clips[0]);
            cameraAnimator.SetTrigger("Slide" + dir);
        } else {
            audioPlayer.PlayOneShot(clips[1]);
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
    }

    public void ResetEnd() {
        sliding = false;
    }

    public void Footstep(int num) {
        audioPlayer.PlayOneShot(clips[1 + num]);
    }

    public void AddHunger(int amount) {
        hunger += amount;
        hungerBar.value += amount * 0.01f;
    }
}
