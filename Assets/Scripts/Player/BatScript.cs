using UnityEngine;

public class BatScript : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;

    private Vector2 direction;
    private Rigidbody2D rb;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("BatScript: Rigidbody2D не найден на объекте!");
            return;
        }
    }

    private void Start()
    {
        Destroy(gameObject, 10f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
}