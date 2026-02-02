using UnityEngine;

public class ManaDrop : MonoBehaviour
{
    public int manaAmount = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TopDownPlayerMovement player = other.GetComponent<TopDownPlayerMovement>();
            if (player != null)
            {
                player.RestoreMana(manaAmount);
            }
            Destroy(gameObject);
        }
    }
}