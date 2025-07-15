using UnityEngine;

public class ExperienceOrb : MonoBehaviour
{
    public int xpValue = 5; // Giá trị XP khi nhặt

    void Start()
    {
        GetComponent<CircleCollider2D>().isTrigger = true; // Đảm bảo là trigger
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0; // Tắt trọng lực
        }
    }
}