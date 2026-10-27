using UnityEngine;

public class IceLanceProjectile : MonoBehaviour
{
    public GameObject iceLanceEffectPrefab;

    private void Start()
    {
        if (iceLanceEffectPrefab != null)
        {
            // Instància l'efecte principal amb la posició i rotació correctes
            Instantiate(iceLanceEffectPrefab, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogError("Falta assignar iceLanceEffectPrefab a IceLanceProjectile!");
        }

        Destroy(gameObject); 
    }
}