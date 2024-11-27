using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; // El robot o agente al que la cámara debe orbitar.
    public float distance = 5f; // Distancia de la cámara al objetivo.
    public float rotationSpeed = 30f; // Velocidad de rotación en grados por segundo.
    public float height = 2f; // Altura de la cámara sobre el objetivo.

    private float currentAngle = 0f; // Ángulo de rotación actual.

    void LateUpdate()
    {
        if (target != null)
        {
            // Incrementar el ángulo de rotación basado en el tiempo y la velocidad.
            currentAngle += rotationSpeed * Time.deltaTime;

            // Convertir el ángulo actual en radianes.
            float radians = currentAngle * Mathf.Deg2Rad;

            // Calcular la posición de la cámara en coordenadas polares.
            float x = Mathf.Sin(radians) * distance;
            float z = Mathf.Cos(radians) * distance;

            // Establecer la posición de la cámara.
            transform.position = target.position + new Vector3(x, height, z);

            // Hacer que la cámara mire siempre al objetivo.
            transform.LookAt(target.position);
        }
    }
}
