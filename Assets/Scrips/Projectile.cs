using Enemy;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] int ProjectileDamage = 5;
    void Start()
    {
       
        Destroy(gameObject, 5f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            
            VidaPlayer player = collision.gameObject.GetComponent<VidaPlayer>();
            if (player != null)
            {
                player.TakeDamage(ProjectileDamage);
            }
            Destroy(gameObject);    
        }



    }


}
