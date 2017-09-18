using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 6.0f;
    public int startingHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Image damageImage;
    public AudioClip deathClip;
    public float timeBetweenBullets = 0.15f;

    private int id;


    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

    private Vector3 movement;
    private Animator anim;
    private Rigidbody playerRigidbody;
    private int floorMask;
    private float camRayLength = 100f;    
    private AudioSource playerAudio;
    private PlayerMovement playerMovement;
    private bool isDead;
    private bool damaged;
    private float timer;
    private float effectsDisplayTime = 0.2f;

    private PlayerShooting playerShooting;

    public GameObject hud;

    public override void OnStartServer()
    {
        //Debug.Log("In OnStartServer");
    }

    public override void OnStartClient()
    {
        //Debug.Log("In OnStartClient");
    }

    public override void OnStartAuthority()
    {
        //Debug.Log("In OnStartAuthority ");
    }

    public void Start()
    {
        id = this.GetInstanceID();
        hud.GetComponent<GameOverManager>().RealWakeUp();
        floorMask = LayerMask.GetMask("Floor");
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAudio = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        currentHealth = startingHealth;
        Debug.Log("End of local player startup");
        playerShooting = GetComponentInChildren<PlayerShooting>();
    }

    public int getID()
    {
        return id;
    }

    public override void OnStartLocalPlayer()
    {
        Camera.main.GetComponent<CameraFollow>().StartCamera(this.gameObject.transform);
    }

    void Awake()
    {


    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Move(h, v);
        Turn();
        Animating(h, v);
    }

    void Move(float h, float v)
    {
        movement.Set(h, 0.0f, v);
        movement = movement.normalized * speed * Time.deltaTime;
        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Turn()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit floorHit;
        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask) == true)
        {
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0f;
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            playerRigidbody.MoveRotation(newRotation);
        }
    }

    void Animating(float h, float v)
    {
        bool walking = h != 0f || v != 0f;
        anim.SetBool("isWalking", walking);
    }
    
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }



        if (damaged)
        {
            damageImage.color = flashColour;
        }
        else
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;
        timer += Time.deltaTime;
        if (Input.GetButton("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
        {
            timer = 0f;
            //Debug.Log("Trying to fire!");
            //Debug.Log(playerShooting);
            CmdShoot();

        }
    }

    [Command]
    public void CmdShoot()
    {
        //Debug.Log("Fire command!");
        //Debug.Log(playerShooting);
        if (playerShooting)
        {
            RpcShoot();
        }
    }

    [ClientRpc]
    public void RpcShoot()
    {
        playerShooting.Shoot();
    }

    public void TakeDamage(int amount)
    {
        damaged = true;

        currentHealth -= amount;

        //healthSlider.value = currentHealth;

        playerAudio.Play();

        if (currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }

    void Death()
    {
        isDead = true;

        playerShooting.DisableEffects();

        anim.SetTrigger("Die");

        playerAudio.clip = deathClip;
        playerAudio.Play();

        playerMovement.enabled = false;
        playerShooting.enabled = false;
    }


    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }
}
