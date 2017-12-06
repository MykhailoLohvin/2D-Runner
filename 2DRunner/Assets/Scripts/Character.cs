using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour {

    [SerializeField]
    private int lives = 5;
    int maxLives = 5;
    [SerializeField]
    private float jumpForce = 9.0f;
    [SerializeField]
    private float pushForce = 6.0f;
    public float speed
    {
        get { return 3.0f; }
    }

    //private Text scoreText;
    private int score = 0;
    private float startCharacterPosition;

    private float pushAbilityCoolDown = 3.0f;
    private float slowAbilityCoolDown = 3.0f;

    GameObject gameUI;
    GameObject endGameUI;

    bool doubleJump = true;

    private bool isGrounded = false;

    private bool alive = true;
    bool stunned = false;

    Rigidbody2D rigidBody2D;
    Animator animator;
    SpriteRenderer spriteRenderer;
    [SerializeField]
    GameObject groundCheckPoint;

    Vector2 startTouchPosition;
    Vector2 endTouchPosition;

    CameraMove mainCamera;
    
    Vector3 characterFreezePosition;

    UIController uiController;

    BoxCollider2D boxCollider;
    bool invulnerable = false;
    
    private CharacterStates State
    {
        get { return (CharacterStates)animator.GetInteger("state"); }
        set { animator.SetInteger("state", (int)value); }
    }

    #region AwakeDestroy
    void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMove>();

        uiController = GameObject.FindGameObjectWithTag("GameControl").GetComponent<UIController>();

        gameUI = GameObject.FindGameObjectWithTag("GameUI");
        endGameUI = GameObject.FindGameObjectWithTag("GameEndUI");

        startCharacterPosition = transform.position.x;

        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        score = 0;

        PlayerPrefs.SetInt("CurrentScore", score);
    }

    void FixedUpdate()
    {
        if (alive)
            GroundCheck();
    }

    void Update()
    {
        if (alive)
        {
            if (!stunned)
            {
                if (isGrounded)
                    State = CharacterStates.idle;

                Walk();
                if (Input.GetButtonDown("Jump") && (isGrounded || doubleJump))
                    Jump();

                if (Input.GetKeyDown(KeyCode.P))
                    PushAbilyty();
                if (Input.GetKeyDown(KeyCode.A))
                    SlowAbility();

                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        startTouchPosition = touch.position;
                    }
                    if (touch.phase == TouchPhase.Ended)
                    {
                        endTouchPosition = touch.position;

                        ActionAfterTouch();
                    }
                }

                if (pushAbilityCoolDown > 0.0f || slowAbilityCoolDown > 0.0f)
                {
                    pushAbilityCoolDown -= Time.deltaTime;
                    slowAbilityCoolDown -= Time.deltaTime;

                    Image[] abilityImages = gameUI.GetComponentsInChildren<Image>();

                    abilityImages[0].fillAmount = Mathf.Clamp(1 - pushAbilityCoolDown / 3.0f, 0.0f, 1.0f);
                    abilityImages[1].fillAmount = Mathf.Clamp(1 - slowAbilityCoolDown / 3.0f, 0.0f, 1.0f);
                }
            }
            SetScore();
        }
        else
        {
            transform.position = characterFreezePosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!invulnerable)
        {
            if(collision.tag == "Saw")
            {
                RecieveDamage(-1);
            }
        }

        if (collision.tag == "Monster" || collision.tag == "CharacterDestroyer")
        {
            StartCoroutine(Dead());
        }
    }
    #endregion

    private void ActionAfterTouch()
    {
        var xDifference = endTouchPosition.x - startTouchPosition.x;
        var yDifference = endTouchPosition.y - startTouchPosition.y;

        if (Mathf.Abs(xDifference) > Mathf.Abs(yDifference))
        {
            if (xDifference > 0)
            {
                PushAbilyty();
            }
            else
            {
                SlowAbility();
            }
        }
        else
        {
            if (yDifference > 0 && (isGrounded || doubleJump))
                Jump();
        }
    }

    IEnumerator Dead()
    {
        alive = false;

        State = CharacterStates.death;
        mainCamera.StopMove();

        characterFreezePosition = transform.position;

        PlayerPrefs.SetInt("CurrentScore", score);

        yield return new WaitForSeconds(1.0f);

        uiController.EndGame();
    }

#region movement
    private void Walk()
    {
        Vector3 direction = new Vector3(1.0f, 0.0f, 0.0f);
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);

        if (isGrounded)
            State = CharacterStates.walk;
    }

    private void Jump()
    {
        if (!isGrounded && doubleJump)
        {
            rigidBody2D.AddForce(transform.up * (jumpForce / 1.5f), ForceMode2D.Impulse);
            doubleJump = false;
        }
        else
            rigidBody2D.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void GroundCheck()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPoint.transform.position, 0.2f);

        isGrounded = colliders.Length > 2;

        if (isGrounded)
            doubleJump = true;

        if (!isGrounded)
            State = CharacterStates.jump;
    }

    private void PushAbilyty()
    {
        if (pushAbilityCoolDown <= 0.0f)
        {
            rigidBody2D.AddForce(transform.right * pushForce, ForceMode2D.Impulse);

            pushAbilityCoolDown = 3.0f;
        }
    }

    private void SlowAbility()
    {
        if (slowAbilityCoolDown <= 0.0f)
        {
            rigidBody2D.AddForce(transform.right * -pushForce, ForceMode2D.Impulse);

            slowAbilityCoolDown = 3.0f;
        }
    }
    #endregion

    private void SetScore()
    {
        score = (int)(transform.position.x - startCharacterPosition);

        uiController.SetScore(score);
        uiController.SetMonsterDistance();
    }

    private void RecieveDamage(int value)
    {
        if (lives > value && lives <= 5)
        {
            lives = Mathf.Clamp(lives + value, 0, 5);
            uiController.OnLivesChange(value);

            if (lives == 0)
            {
                StartCoroutine(Dead());
                return;
            }

            StartCoroutine(Stunned());
        }
        else
        {
            StartCoroutine(Dead());
        }
    }

    IEnumerator Invulnerable()
    {
        invulnerable = true;
        spriteRenderer.color = new Color(255.0f, 255.0f, 255.0f, 50.0f);

        yield return new WaitForSeconds(2.0f);

        invulnerable = false;
        spriteRenderer.color = new Color(255.0f, 255.0f, 255.0f, 255.0f);
    }

    IEnumerator Stunned()
    {
        stunned = true;
        State = CharacterStates.stun;

        yield return new WaitForSeconds(1.0f);

        stunned = false;
        State = CharacterStates.walk;

        StartCoroutine(Invulnerable());
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}

public enum CharacterStates
{
    idle,
    walk,
    jump,
    death,
    stun
}

