using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int dmg;

    public float lifetime = 3.0f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("BorderBullet"))
        {
            Destroy(gameObject);
        }
    }
}
