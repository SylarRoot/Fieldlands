using UnityEngine;

public class Projectile_P : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 3.5f); // Destrueix el projectil després de 5 segons
        

    }


    private void OnCollisionEnter(Collision collision)
    {
       
        // Comprovem si el projectil ha col·lisionat amb un enemic
        if (collision.gameObject.CompareTag("Enemy_Fire") ||
            collision.gameObject.CompareTag("Enemy_Melee_Fire") ||
            collision.gameObject.CompareTag("Enemy_Ice") ||
            collision.gameObject.CompareTag("Enemy_Melee_Ice") ||
            collision.gameObject.CompareTag("Enemy_Lightning") ||
            collision.gameObject.CompareTag("Enemy_Melee_Lightning") ||
            collision.gameObject.CompareTag("Boss_Fire") ||
            collision.gameObject.CompareTag("Boss_Ice") ||
            collision.gameObject.CompareTag("Boss_Lightning"))
        {
            Destroy(gameObject); // Destrueix el projectil immediatament
        }
    }
}
