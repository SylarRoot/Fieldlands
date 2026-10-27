using System.Collections.Generic;
using UnityEngine;

public class AutoAim : MonoBehaviour
{
    [Header("Auto-Aim Settings")]
    [SerializeField] private bool autoAimEnabled = true;
    [SerializeField] private float detectionRadius;         // Radi de detecció d'enemics
    [SerializeField] private float maxAimAngle;             // Angle màxim per considerar un enemic
    [SerializeField] private LayerMask enemyLayerMask;      
    [SerializeField] private Transform playerCamera;        // Referència a la càmera del jugador
    [SerializeField] private float aimAssistStrength;       // Força de l'assistència (0-1)

    // Per a visualització a l'editor
    [SerializeField] private bool showDebugLines = false;
    [SerializeField] private Color debugLineColor = Color.red;

    // Objecte actualment enfocat
    private Transform currentTarget;

    private void Update()
    {
        if (autoAimEnabled)
        {
            FindBestTarget();
        }
        else
        {
            currentTarget = null;
        }

        if (showDebugLines && currentTarget != null)
        {
            Debug.DrawLine(transform.position, currentTarget.position, debugLineColor);
        }
    }

    private void FindBestTarget()
    {
        // Cerca tots els enemics dins del radi de detecció
        Collider[] potentialTargets = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayerMask);

        // Variables pel millor objectiu
        float closestAngle = maxAimAngle;
        Transform bestTarget = null;

        foreach (Collider collider in potentialTargets)
        {
            // Comprova si té algun dels tags d'enemic
            if (IsEnemyTag(collider.tag))
            {
                Vector3 directionToTarget = (collider.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(playerCamera.forward, directionToTarget);

                // Si és dins l'angle màxim i és el més proper al centre de la mira
                if (angle < closestAngle)
                {
                    // Verifica que no hi hagi obstacles pel camí
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, directionToTarget, out hit, detectionRadius))
                    {
                        if (IsEnemyTag(hit.collider.tag))
                        {
                            closestAngle = angle;
                            bestTarget = collider.transform;
                        }
                    }
                }
            }
        }

        currentTarget = bestTarget;
    }

    // Verifica si el tag pertany a algun dels tipus d'enemic
    private bool IsEnemyTag(string tag)
    {
        return tag == "Enemy_Fire" || tag == "Enemy_Melee_Fire" ||
               tag == "Enemy_Ice" || tag == "Enemy_Melee_Ice" ||
               tag == "Enemy_Lightning" || tag == "Enemy_Melee_Lightning" ||
               tag == "Boss_fire" || tag == "Boss_ice" || tag == "Boss_lightning";

    }

    // Mètode per obtenir la direcció de tir ajustada amb l'autoapuntat
    public Vector3 GetAdjustedShootDirection(Vector3 originalDirection)
    {
        if (!autoAimEnabled || currentTarget == null)
        {
            return originalDirection;
        }

        // Calcula la direcció cap a l'objectiu
        Vector3 directionToTarget = (currentTarget.position - transform.position).normalized;

        // Interpola entre la direcció original i la direcció cap a l'objectiu
        Vector3 adjustedDirection = Vector3.Lerp(originalDirection, directionToTarget, aimAssistStrength);

        return adjustedDirection.normalized;
    }

    // Mètode per verificar si hi ha un objectiu vàlid
    public bool HasTarget()
    {
        return currentTarget != null;
    }

    // Mètode per obtenir l'objectiu actual
    public Transform GetCurrentTarget()
    {
        return currentTarget;
    }
}
