using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Para manejar UI si usas texto en pantalla

public class LightInteraction : MonoBehaviour
{
    [Header("Light Settings")]
    public Light targetLight; // La luz que queremos encender/apagar
    public KeyCode interactionKey = KeyCode.E; // Tecla para interactuar

    [Header("UI")]
    public GameObject interactionTextUI; // Texto que aparece en pantalla al acercarse

    private bool isPlayerNearby = false; // Para saber si el jugador está en rango
    private bool isLightOn = false; // Estado actual de la luz

    private void Start()
    {
        // Asegúrate de que el texto de interacción esté oculto al inicio
        if (interactionTextUI != null)
            interactionTextUI.SetActive(false);

        // Verifica que la luz esté inicializada
        if (targetLight != null)
            targetLight.enabled = false; // Comenzamos con la luz apagada
    }

    private void Update()
    {
        // Si el jugador está cerca y presiona la tecla de interacción
        if (isPlayerNearby && Input.GetKeyDown(interactionKey))
        {
            ToggleLight();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detectar si el jugador entra al rango
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;

            // Mostrar texto de interacción si está configurado
            if (interactionTextUI != null)
                interactionTextUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Detectar si el jugador sale del rango
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            // Ocultar texto de interacción
            if (interactionTextUI != null)
                interactionTextUI.SetActive(false);
        }
    }

    private void ToggleLight()
    {
        // Alternar estado de la luz
        if (targetLight != null)
        {
            isLightOn = !isLightOn;
            targetLight.enabled = isLightOn; // Encender o apagar la luz
        }
    }
}