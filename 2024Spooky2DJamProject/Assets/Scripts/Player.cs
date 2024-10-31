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
    private Vector2 screenCenter;
    private GameObject dupeScene;
    private string[] directions = {"Up", "Right", "Right", "Down", "Right", "Up", "Up", "Right", "Up", "Up"};
    private GameObject screens;
    private int currScene = 0;
    private bool facingRight;
    private bool sliding;
    private string transitionDir;

    public int moveSpeed = 6;
    public int hunger = 100;
    public float hungerPerSecond = 1.5f;
    private float hungerTickLength;
    private float hungerTime;
    private float creepyLength;
    private float creepyTime;
    private float screenSize = 5;
    private bool wrongWayReset;
    private GameObject[] scenes;
    void Start() {
        scenes = new GameObject[10];
        for (int i = 0; i < 10; i++) {
            scenes[i] = GameObject.Find("Scene " + (i+1));
            if (i > 0) {
                scenes[i].SetActive(false);
            }
        }
        screens = GameObject.Find("Screens");
        screenCenter = new(0,0);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cameraAnimator = Camera.main.GetComponent<Animator>();
        audioPlayer = GameObject.FindWithTag("SFX").GetComponent<AudioSource>();
        hungerBar = GameObject.FindWithTag("HungerBar").GetComponent<Slider>();
        hunger = 100;
        hungerPerSecond = 1.5f;
        hungerTickLength = 1f / hungerPerSecond;
        hungerTime = 0f;
        screenSize = 5;
        wrongWayReset = false;
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
        if (wrongWayReset) { //FIX THIS
            Vector3 dir = DirToVect(transitionDir) * 10;
            transform.position -= dir;
            Camera.main.transform.position -= dir;
            Destroy(dupeScene);
            wrongWayReset = false;
        }
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
            //audioPlayer.PlayOneShot(clips[4]);
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
            //boundary checks for screen transition
            if (playerPos.x <= screenCenter.x - screenSize) {
                SceneChange("Left");
            } else if (playerPos.x >= screenCenter.x + screenSize) {
                SceneChange("Right");
            } else if (playerPos.y <= screenCenter.y - screenSize) {
                SceneChange("Down");
            } else if (playerPos.y >= screenCenter.y + screenSize) {
                SceneChange("Up");
            } else {
                //keep player within screen bounds
                playerPos.x = Mathf.Clamp(playerPos.x, screenCenter.x - screenSize, screenCenter.x + screenSize);
                playerPos.y = Mathf.Clamp(playerPos.y, -1 * screenCenter.y - screenSize, screenCenter.y + screenSize);
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
            if (currScene == 9) {
                SceneManager.LoadScene("WinScene");
            } else {
                scenes[currScene + 1].SetActive(true);
                audioPlayer.PlayOneShot(clips[0]);
                cameraAnimator.SetTrigger("Slide" + dir);
                Darkness darkness = GameObject.Find("Darkness").GetComponent<Darkness>();
                darkness.Darken();
            }
        } else {
            hunger -= 5;
            hungerBar.value -= 0.05f;
            audioPlayer.PlayOneShot(clips[1]);
            dupeScene = Instantiate(scenes[currScene], GameObject.Find("Screens").transform); //move to global
            dupeScene.transform.position = screenCenter + DirToVect(transitionDir) * 10;
            cameraAnimator.SetTrigger("Slide" + dir);
        }
    }

    public void SlideEnd() {
        cameraAnimator.SetTrigger("Idle"); //is this necessary?
        if (dupeScene) {
            wrongWayReset = true;
        } else {
            scenes[currScene].SetActive(false);
            screenCenter += DirToVect(transitionDir) * 10;
            currScene++;
        }
        sliding = false;
    }

    // public void ResetPosition() {
    //     Vector2 newPos = screenBounds;
    //     if (transitionDir == "Up") {
    //         newPos.x -= screenSize;
    //         newPos.y -= screenSize * 2f;
    //     } else if (transitionDir == "Down") {
    //         newPos.x -= screenSize;
    //     } else if (transitionDir == "Left") {
    //         newPos.y -= screenSize;
    //     } else {
    //         newPos.x -= screenSize * 2f;
    //         newPos.y -= screenSize;
    //     }
    //     transform.position = newPos;
    // }

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

    public Vector2 DirToVect(string dir) {
        if (dir == "Up") {
            return new(0,1);
        } else if (dir == "Down") {
            return new(0,-1);
        } else if (dir == "Right") {
            return new(1,0);
        } else {
            return new(-1,0);
        }
    }
}
