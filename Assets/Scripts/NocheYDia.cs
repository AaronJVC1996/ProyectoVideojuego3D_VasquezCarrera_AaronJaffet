using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NocheYDia : MonoBehaviour
{
      [Header("Cycle Settings")]
    public float dayDuration = 60f;
    public float tiltAngle = 23.5f;

    [Header("Lights")]
    public Light sunLight;
    public Light moonLight;
    public float maxIntensity = 2f;
    public float minIntensity = 0f;

    [Header("Background Settings")]
    public Camera mainCamera;       // Cámara principal
    public Color dayColor = Color.cyan;  // Color del cielo durante el día
    public Color nightColor = Color.black; // Color del cielo durante la noche
    public Material daySkybox;      // Skybox para el día
    public Material nightSkybox;    // Skybox para la noche
    public float maxExposure = 1f;  // Exposición máxima durante el día
    public float minExposure = 0.06f;  // Exposición mínima durante la noche

    [Header("Skybox Rotation")]
    public float rotationSpeed = 1f; // Velocidad de rotación de las nubes

    void Start()
    {
        transform.rotation = Quaternion.Euler(tiltAngle, 0, 0);

        if (mainCamera == null)
            mainCamera = Camera.main; // Asigna la cámara principal si no está configurada

        // Asigna el skybox inicial
        RenderSettings.skybox = daySkybox;
        SetSkyboxExposure(maxExposure); // Exposición máxima al inicio
    }

    void Update()
    {
        float rotationSpeedY = 360f / dayDuration * Time.deltaTime;
        transform.Rotate(Vector3.up, rotationSpeedY);

        AdjustLighting();
        UpdateBackground();
        RotateSkybox();  // Agregar rotación continua del skybox
    }

    private void AdjustLighting()
    {
        float angle = transform.eulerAngles.y;

        if (angle >= 0f && angle < 180f)
        {
            sunLight.intensity = CalculateIntensity(angle, 0f, 180f);
            moonLight.intensity = minIntensity;
        }
        else
        {
            moonLight.intensity = CalculateIntensity(angle, 180f, 360f);
            sunLight.intensity = minIntensity;
        }
    }

    private float CalculateIntensity(float angle, float startAngle, float endAngle)
    {
        float midPoint = (startAngle + endAngle) / 2f;
        float distanceToMid = Mathf.Abs(angle - midPoint);
        float normalized = 1f - (distanceToMid / (midPoint - startAngle));
        return Mathf.Lerp(minIntensity, maxIntensity, normalized);
    }

    private void UpdateBackground()
    {
        // Ajusta el color del fondo de la cámara según la intensidad del Sol y la Luna
        float sunIntensityNormalized = Mathf.InverseLerp(minIntensity, maxIntensity, sunLight.intensity);
        float moonIntensityNormalized = Mathf.InverseLerp(minIntensity, maxIntensity, moonLight.intensity);

        // Mezcla entre el color del día y la noche
        Color backgroundColor = Color.Lerp(nightColor, dayColor, sunIntensityNormalized);
        backgroundColor = Color.Lerp(backgroundColor, nightColor, moonIntensityNormalized);

        // Actualiza el fondo de la cámara
        mainCamera.backgroundColor = backgroundColor;

        // Cambiar el Skybox basado en la intensidad de la luz del sol o la luna
        if (sunLight.intensity > 0) // Día
        {
            RenderSettings.skybox = daySkybox;
            // Ajusta la exposición del Skybox para simular el oscurecimiento del cielo durante el día
            SetSkyboxExposure(Mathf.Lerp(minExposure, maxExposure, sunIntensityNormalized));
        }
        else // Noche
        {
            RenderSettings.skybox = nightSkybox;
            SetSkyboxExposure(minExposure); // Exposición mínima durante la noche
        }
    }

    // Método para actualizar la exposición del Skybox
    private void SetSkyboxExposure(float exposure)
    {
        // Verifica que el material del Skybox sea un material de tipo Skybox/Panoramic
        if (RenderSettings.skybox.HasProperty("_Exposure"))
        {
            RenderSettings.skybox.SetFloat("_Exposure", exposure);
        }
    }

    // Método para hacer rotar el skybox lentamente
    private void RotateSkybox()
    {
        // Obtiene el valor de la rotación actual del skybox
        float currentRotation = RenderSettings.skybox.GetFloat("_Rotation");
        
        // Aumenta la rotación en cada frame para simular el movimiento de las nubes
        currentRotation += rotationSpeed * Time.deltaTime;
        
        // Establece el nuevo valor de rotación
        RenderSettings.skybox.SetFloat("_Rotation", currentRotation);
    }
}