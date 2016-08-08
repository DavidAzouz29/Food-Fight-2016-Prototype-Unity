﻿/// ----------------------------------
/// <summary>
/// Name: PlayerController.cs
/// Author: David Azouz
/// Date Created: 20/06/2016
/// Date Modified: 6/2016
/// ----------------------------------
/// Brief: Player Controller class that controls the player.
/// viewed: https://unity3d.com/learn/tutorials/projects/roll-a-ball/moving-the-player
/// http://wiki.unity3d.com/index.php?title=Xbox360Controller
/// http://answers.unity3d.com/questions/788043/is-it-possible-to-translate-an-object-diagonally-a.html
/// *Edit*
/// - Player state machine - David Azouz 20/06/2016
/// - Player moving at a 45 degree angle - David Azouz 20/06/2016
/// TODO:
/// - More than one player added - /6/2016
/// - 
/// </summary>
/// ----------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public const uint MAX_PLAYERS = PlayerManager.MAX_PLAYERS;

    // PRIVATE VARIABLES [MenuItem ("MyMenu/Do Something")]
    [Header("Movement")]
    public float playerSpeed = 10.0f;
    public float speedBoost = 6.0f;
    public bool m_Moving = false;

    [Tooltip("This will change at runtime.")]
    [Header("Health")]
    public int hitsBeforeDeath = 5;
    public int health = 100;
    
    // these will change for each player 
    [Header("KeyBinds")] 
    public string verticalAxis = "_Vertical";
    public string horizontalAxis = "_Horizontal";
	public string Attack1 = "Attack1";
	public string Attack2 = "Attack2";
    public string Jump = "_Jump";
    [HideInInspector]
    public string m_PlayerTag = "NoPlayerAttached";

    Animator animator;
    public PlayerShooting m_ShootingManager;

    //public AudioSource dizzyBirds;
    //public GameManager 
    //[Header("Weapon")]
    //public GameObject r_weapon;
    //public GameObject r_gameOverPanel;
    //public GameObject r_bombExplosionParticleEffect;
    //choosing player states
    [HideInInspector]
    public enum E_PLAYER_STATE
    {
        E_PLAYER_STATE_ALIVE,
        E_PLAYER_STATE_BOSS, //TODO: add more?
        E_PLAYER_STATE_TEAMUP,
        E_PLAYER_STATE_DEAD,

        E_PLAYER_STATE_COUNT,
    };
    public E_PLAYER_STATE m_eCurrentPlayerState;

    // what class the player is
    [HideInInspector]
    public enum E_CLASS_STATE
    {
        E_CLASS_STATE_ROCKYROAD,
        E_CLASS_STATE_BROCCOLION,
        E_CLASS_STATE_WATERMELOMON,
        E_CLASS_STATE_KARATEA,
        E_CLASS_STATE_PATTYCAKE,
        E_CLASS_STATE_CAUILILION,
        E_CLASS_STATE_ROCKMELOMON,

        E_PLAYER_STATE_COUNT,
    };
    public E_CLASS_STATE m_eCurrentClassState;

    // PRIVATE VARIABLES
    // A way to identidy players
    [SerializeField] private uint m_playerID = 0;
    private float fRot = 0.2f;
    bool isPaused = false;
    // Health
    private int healthDeduct = 0;

    // used for jumping
    Rigidbody rb;
    //private float m_JumpHeight = 5;
    bool isOnGround; // set to true if we are on the ground
    public float fJumpForce = 12.0f;
    public float fGravity = 9.8f;
    float fJumpForceMax = 24.0f;// *2;
    private Vector3 m_PreviousPos;
    private float charYvel;

    // Use this for initialization
    void Start ()
    {
		//setting our current state to alive
        m_eCurrentPlayerState = E_PLAYER_STATE.E_PLAYER_STATE_ALIVE;

        verticalAxis = "_Vertical";
        horizontalAxis = "_Horizontal";
        Attack1 = "_Attack1";
        Attack2 = "_Attack2";
        Jump = "_Jump";

        switch (m_eCurrentClassState)
        {
            case E_CLASS_STATE.E_CLASS_STATE_ROCKYROAD:
                {
                    m_PlayerTag = "Rockyroad";
                    break;
                }
            case E_CLASS_STATE.E_CLASS_STATE_BROCCOLION:
                {
                    m_PlayerTag = "Brocolion";
                    break;
                }
            case E_CLASS_STATE.E_CLASS_STATE_WATERMELOMON:
                {
                    m_PlayerTag = "Watermelomon";
                    break;
                }
            case E_CLASS_STATE.E_CLASS_STATE_KARATEA:
                {
                    m_PlayerTag = "Karatea";
                    break;
                }
            case E_CLASS_STATE.E_CLASS_STATE_CAUILILION:
                {
                    m_PlayerTag = "Caulilion";
                    break;
                }
            default:
                {
                    Debug.LogError("No Character Attached");
                    break;
                }
        }

        // Loops through our players and assigns variables for input from different controllers
        for (uint i = 0; i < MAX_PLAYERS; ++i)
        {
            if (m_playerID == i)
            {
                verticalAxis = "P" + (i + 1) + verticalAxis; //"_Vertical";
                horizontalAxis = "P" + (i + 1) + horizontalAxis; // "_Horizontal";
                Attack1 = "P" + (i + 1) + Attack1;
                Attack2 = "P" + (i + 1) + Attack2;
                Jump = "P" + (i + 1) + Jump;
            }
        }
        //m_ShootingManager.SetFire(Fire);
        //TODO: healthBars = FindObjectOfType<healthBar> ();
        healthDeduct = health / hitsBeforeDeath;
        animator = GetComponent<Animator>(); //GetComponentInChildren<Animator> ();
        rb = GetComponent<Rigidbody>();
        //jump
        //fMovementSpeedSlowDown = fMovementSpeed - 2.0f;
        fJumpForceMax = fJumpForce;// *2;
        m_PreviousPos = transform.position;
    }

    // Should be used for Physics calculations
    void FixedUpdate()
    {
        // Jumping
        // if we're on or close to the ground
        if ((m_PreviousPos.y - transform.position.y) < 0.3f)
        {
            ///rb.velocity = new Vector3(0, 0.1f, 0);
            //fJumpForce = fJumpForceMax;
            isOnGround = true;
            charYvel = 0;
            animator.SetBool("Jumping", false);
        }
        // Falling
        else
        {
            charYvel = rb.velocity.y;
            //rb.velocity = new Vector3(0, charYvel += Physics.gravity.y * Time.deltaTime, 0); //m_characterYvelocity += m_playerGravity * a_deltaTime;
            isOnGround = false;
        }
        if (Input.GetButton(Jump) && isOnGround)
        {
            //rb.MovePosition(rb.position + transform.up * fJumpForce * Time.deltaTime);
            charYvel = fJumpForce;
            charYvel -= fGravity * Time.deltaTime;
            rb.MovePosition(rb.position + transform.up * charYvel * Time.deltaTime);
            isOnGround = false;
            animator.SetBool("Jumping", true);
            switch (m_eCurrentClassState)
            {
                case E_CLASS_STATE.E_CLASS_STATE_ROCKYROAD:
                    {
                        animator.CrossFade("Rocky_Jump", 0);
                        break;
                    }
                case E_CLASS_STATE.E_CLASS_STATE_BROCCOLION:
                    {
                        animator.CrossFade("Brocco_Leap", 0);
                        break;
                    }
                case E_CLASS_STATE.E_CLASS_STATE_WATERMELOMON:
                    {
                        animator.CrossFade("Watermelomon_Jump", 0);
                        break;
                    }
                case E_CLASS_STATE.E_CLASS_STATE_KARATEA:
                    {
                        animator.CrossFade("", 0);
                        break;
                    }
                    // CauiliLION is a skinned version of BroccoLION
                case E_CLASS_STATE.E_CLASS_STATE_CAUILILION:
                    {
                        animator.CrossFade("Brocco_Leap", 0);
                        break;
                    }
                default:
                    {
                        Debug.LogError("Character animation not set up");
                        break;
                    }
            }

            animator.SetBool("Idling", false);
            animator.SetBool("Walking", false);
            //rb.velocity = new Vector3(0, -fJumpForce * Time.deltaTime, 0); //transform.velocity.y -= fJumpForce * a_deltaTime;// fMovementSpeed * a_deltaTime;
            //fJumpForceMax -= 1.0f;
            // TODO: further research asymptotes
            //float x = velocity.y;
            // x^2
            //float fJumpAsymptote = ((x * x) - 3 * x) / ((2 * x) - 2);
        }
        m_PreviousPos = transform.position;
    }

    // Update is called once per frame
    void Update ()
    {

        if (Input.GetButton("Pause"))
        {
            // equal to the state we're not (true will equal not true (which is false))
            //isPaused = !isPaused;
            //Time.timeScale = 0.01f;
        }
        // if we're not paused and our timescale is modified
        if (!isPaused && Time.timeScale != 1)
        {
            isPaused = true;
            //Time.timeScale = 1;
        }
        if (isPaused == true)
        {
            if(Input.anyKey)
            {
                //Application.UnloadLevel(0);
                //Application.LoadLevel(0);
            }
        }

        //creating a variable that gets the input axis
        float moveHorizontal = Input.GetAxis(horizontalAxis);
        float moveVertical = Input.GetAxis(verticalAxis);

        // Movement
        if ((moveHorizontal < -fRot || moveHorizontal > fRot ||
                  moveVertical < -fRot || moveVertical > fRot) && isPaused == false)
        {
            m_Moving = true;
            // TOOD: rb.AddForce(Vector3.up);
            Vector3 movementDirection = new Vector3 (moveHorizontal, 0.0f, moveVertical);
            movementDirection = Quaternion.Euler(0, 45, 0) * movementDirection;
            Vector3 pos = transform.position + movementDirection * playerSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp (transform.position, pos, 0.2f);
            transform.forward = new Vector3(-moveVertical, 0.0f, moveHorizontal);
            transform.forward = Quaternion.Euler(0, -45, 0) * transform.forward;
        }
        // we're not moving so play the idle animation
        else
        {
            m_Moving = false;
        }

        if (health <= 0)
        {
            m_eCurrentPlayerState = E_PLAYER_STATE.E_PLAYER_STATE_DEAD;
            //DO STUFF
        }

        //Switches between player states
        #region Player States
        switch (m_eCurrentPlayerState)
        {
            //checks if the player is alive
            case E_PLAYER_STATE.E_PLAYER_STATE_ALIVE:
                {
                    //gameObject.GetComponent<BossBlobs>().enabled = false;
                    //r_weapon.SetActive(false);
                    //Debug.Log("Alive!");
                    break;
                }
            case E_PLAYER_STATE.E_PLAYER_STATE_BOSS:
                {
                    //gameObject.GetComponent<BossBlobs>().enabled = true;
                    //r_weapon.SetActive(true);
                    //Debug.Log("Boss");
                    break;
                }
            /*case E_PLAYER_STATE.E_PLAYER_STATE_TEAMUP:
                {
                    // if player 'A' and 'B's' states
                    switch(m_eCurrentClassState)
                    {
                        case E_CLASS_STATE.E_CLASS_STATE_ROCKYROAD:
                            {

                            }
                    }
                    m_playerID
                } */
            //if player is dead
            case E_PLAYER_STATE.E_PLAYER_STATE_DEAD:
                {
                    //gameObject.GetComponent<BossBlobs>().enabled = false;
                    // Particle effect bomb (explosion)
                    //r_bombExplosionParticleEffect.SetActive(true);
                    // actions to perform after a certain time
                    uint uiBombEffectTimer = 2;
                    Invoke("BombEffectDead", uiBombEffectTimer);
//					c_death.CrossFade("Death");

                    Debug.Log("Dead :(");
                    break;
                }
            default:
                {
                    Debug.LogError("No State Chosen!");
                    break;
                }
        }
        #endregion
	}

    void BombEffectDead()
    {
        //r_weapon.SetActive(false);
        Destroy(this.gameObject);
        //r_gameOverPanel.SetActive(true); 
        Time.timeScale = 0.00001f;
        // After three seconds, return to menu
        Invoke("ReturnToMenu", 1);
        Debug.Log("Bomb Effect");
    }

    void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    // this is the function that should be used
    public void SetPlayerState(E_PLAYER_STATE a_ePlayerState)
    {
        m_eCurrentPlayerState = a_ePlayerState;
    }

    public uint GetPlayerID()
    {
        return m_playerID;
    }

    public void SetPlayerID(uint a_uiPlayerID)
    {
        m_playerID = a_uiPlayerID; 
    }


    void OnCollisionEnter(Collision a_collision)
    {
        if (a_collision.gameObject.tag == "Weapon")
        {
            Debug.Log("PC: HIT");
            health -= healthDeduct; //20?
            //dizzyBirds.Play();

            float healthFraction = 1.0f - (float)health / 100;
            healthFraction = Mathf.Lerp(0, 5, healthFraction);
            int healthImageID = Mathf.FloorToInt(healthFraction);

            //healthBars.healthHit (m_playerID, healthImageID);
        }

        // make jump work
        if (a_collision.transform.tag == "Bench")
        {
            isOnGround = true;
        }
    }
}
