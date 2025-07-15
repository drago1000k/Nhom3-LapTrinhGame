using UnityEngine;

public class HealPickup : MonoBehaviour
{
    public float healAmount = 20f; // Số máu hồi khi thu thập

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.Heal(healAmount); // Gọi phương thức Heal từ Player
            Destroy(gameObject); // Xóa healPickUp sau khi thu thập
        }
    }
}