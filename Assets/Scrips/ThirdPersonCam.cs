using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Vector3 offset = new Vector3(0, 2, -4);

    [Header("Settings")]
    public float smoothSpeed = 5f;
    public float rotationSpeed = 5f;
    public float cameraTiltAngle = 10f;
    public float collisionBuffer = 0.2f; // Distància extra per evitar que la càmera toqui els obstacles
    public LayerMask collisionLayers; // Capes que poden bloquejar la càmera
    public float minCameraHeight = 1.0f; // Altura mínima de la càmera per evitar veure només les cames

    private void LateUpdate()
    {
        if (player == null) return;

        // Posició desitjada sense obstacles
        Vector3 targetPosition = player.position + player.TransformDirection(offset);

        // Direcció entre el jugador i la posició desitjada de la càmera
        Vector3 direction = (targetPosition - player.position).normalized;
        float maxDistance = offset.magnitude;

        RaycastHit hit;
        if (Physics.SphereCast(player.position, 0.3f, direction, out hit, maxDistance + collisionBuffer, collisionLayers))
        {
            // Si hi ha un obstacle, ajustem la distància per posar la càmera just davant d'aquest obstacle
            float distance = Mathf.Clamp(hit.distance - collisionBuffer, 0.5f, maxDistance);
            targetPosition = player.position + direction * distance;
        }

        // Assegura que la càmera no estigui per sota de l'altura mínima
        targetPosition.y = Mathf.Max(targetPosition.y, player.position.y + minCameraHeight);

        // Suavitza la posició
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // Rotació suau per mirar cap al jugador
        Vector3 lookAtPosition = player.position + Vector3.up * 1.5f;
        Quaternion lookRotation = Quaternion.LookRotation(lookAtPosition - transform.position);
        lookRotation *= Quaternion.Euler(cameraTiltAngle, 0, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }
}
