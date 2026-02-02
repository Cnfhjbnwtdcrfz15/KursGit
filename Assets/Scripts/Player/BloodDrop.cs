using UnityEngine;

public class BloodDrop : MonoBehaviour
{
    public int healthAmount = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TopDownPlayerMovement player = other.GetComponent<TopDownPlayerMovement>();
            if (player != null)
            {
                player.RestoreHealth(healthAmount);
            }
            Destroy(gameObject);
        }
    }
}