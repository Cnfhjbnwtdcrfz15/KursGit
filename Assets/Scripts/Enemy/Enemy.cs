using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public int health = 50;

    public GameObject manaDropPrefab;
    public GameObject bloodDropPrefab;

    [Header("Мана")]
    public int minManaDrops = 1;
    public int maxManaDrops = 2;

    [Header("Кровь")]
    public int minBloodDrops = 0;
    public int maxBloodDrops = 1;

    public float dropRadius = 1f;

    public AudioClip hitSound;
    public AudioClip DethSound;
    public AudioSource audioSource;
    private Animator animator;

    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }

    public void TakeDamage(int amount)
    {
        PlayHitEffect();

        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void PlayHitEffect()
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    void Die()
    {
        if (DethSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(DethSound);
        }
        int manaCount = Random.Range(minManaDrops, maxManaDrops + 1);
        for (int i = 0; i < manaCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * dropRadius;
            Instantiate(manaDropPrefab, (Vector2)transform.position + offset, Quaternion.identity);
        }

        int bloodCount = Random.Range(minBloodDrops, maxBloodDrops + 1);
        for (int i = 0; i < bloodCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * dropRadius;
            Instantiate(bloodDropPrefab, (Vector2)transform.position + offset, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}