using UnityEngine;
using UnityEngine.UI;

public class TopDownPlayerMovement : MonoBehaviour
{
    // === ССЫЛКИ НА КОМПОНЕНТЫ ===
    [Header("UI")]
    public Slider healthSlider;
    public Slider manaSlider;
    public GameObject gameOverCanvas;

    [Header("Аудио")]
    public AudioSource audioSource;
    public AudioClip deathSound;
    public AudioClip fireSound;
    public AudioClip takeDamageSound;

    [Header("Движение")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("Стрельба")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootCooldown = 0.2f;
    private float lastShotTime = 0f;

    [Header("Способность: мыши")]
    public GameObject mouseProjectilePrefab;
    public int mouseBurstCount = 8;
    public float mouseBurstCooldown = 5f;
    public float mouseBurstManaCost = 30f;
    private float lastBurstTime = 0f;

    [Header("Ресурсы")]
    public float maxMana = 100f;
    public float manaCost = 10f;
    public float manaRegenRate = 5f;
    public float currentMana;

    public int maxHealth = 100;
    public int currentHealth;

    [Header("Боевая система")]
    public float damageInterval = 1f;
    private float lastDamageTime = -10f;
    private int isTouchingEnemy = 0;

    [Header("Анимация")]
    [SerializeField] private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        if (firePoint == null)
            firePoint = transform;

        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    void Update()
    {
        UpdateUI();
        HandleMovementInput();
        RegenerateMana();
        HandleShooting();
        HandleMouseBurst();
        HandleDamageOverTime();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void UpdateUI()
    {
        if (healthSlider != null)
            healthSlider.value = (float)currentHealth / maxHealth;

        if (manaSlider != null)
            manaSlider.value = currentMana / maxMana;
    }

    void HandleMovementInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        bool isMoving = movement != Vector2.zero;
        animator.SetBool("isRunning", isMoving);
    }

    void MovePlayer()
    {
        if (movement != Vector2.zero)
        {
            rb.velocity = movement.normalized * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    void RegenerateMana()
    {
        if (currentMana < maxMana)
        {
            currentMana += manaRegenRate * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, maxMana);
        }
    }

    void HandleShooting()
    {
        if (Input.GetMouseButton(0) &&
            Time.time >= lastShotTime + shootCooldown &&
            currentMana >= manaCost)
        {
            Shoot();
            currentMana -= manaCost;
            lastShotTime = Time.time;
        }
    }

    void HandleMouseBurst()
    {
        if (Input.GetMouseButtonDown(1) &&
            Time.time >= lastBurstTime + mouseBurstCooldown &&
            currentMana >= mouseBurstManaCost)
        {
            SpawnMouseBurst();
            currentMana -= mouseBurstManaCost;
            lastBurstTime = Time.time;
        }
    }

    void HandleDamageOverTime()
    {
        if (isTouchingEnemy > 0 && Time.time >= lastDamageTime + damageInterval)
        {
            TakeDamage(10);
            lastDamageTime = Time.time;
        }
    }

    void Shoot()
    {
        animator.SetTrigger("Shoot");
        PlaySound(fireSound);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - (Vector2)firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        BatScript bulletComp = bullet.GetComponent<BatScript>();
        if (bulletComp != null)
        {
            bulletComp.SetDirection(direction);
        }
    }

    void SpawnMouseBurst()
    {
        PlaySound(fireSound);

        float angleStep = 360f / mouseBurstCount;
        for (int i = 0; i < mouseBurstCount; i++)
        {
            float angle = angleStep * i;
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            GameObject mouse = Instantiate(mouseProjectilePrefab, transform.position, Quaternion.identity);
            BatScript mouseScript = mouse.GetComponent<BatScript>();
            if (mouseScript != null)
            {
                mouseScript.SetDirection(direction);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage * isTouchingEnemy;
        PlaySound(takeDamageSound);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        PlaySound(deathSound);

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isTouchingEnemy++;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isTouchingEnemy--;
        }
    }
    public void RestoreMana(int amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
    }

    public void RestoreHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}