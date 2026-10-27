using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public float projectileUpward = 0.1f;
    public GameObject explosionPrefab;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        // Calcula la direcci� del dispar 
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Vector3 targetPoint = ray.origin + ray.direction * 100f;
        Vector3 shootDirection = (targetPoint - transform.position).normalized;

        // Combina amb la component vertical
        Vector3 finalDirection = (shootDirection + Vector3.up * projectileUpward).normalized;

        // Assigna velocitat I rotaci�
        rb.linearVelocity = finalDirection * speed;
        transform.rotation = Quaternion.LookRotation(finalDirection);

        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy_melee") || 
            collision.gameObject.CompareTag("Enemy_Fire") || 
            collision.gameObject.CompareTag("Enemy_Melee_Fire") || 
            collision.gameObject.CompareTag("Enemy_Ice") || 
            collision.gameObject.CompareTag("Enemy_Melee_Ice") || 
            collision.gameObject.CompareTag("Enemy_Lightning") || 
            collision.gameObject.CompareTag("Enemy_Melee_Lightning") ||
            collision.gameObject.CompareTag("Boss_Fire") || 
            collision.gameObject.CompareTag("Boss_Ice") ||
            collision.gameObject.CompareTag("Boss_Lightning"))
    {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}