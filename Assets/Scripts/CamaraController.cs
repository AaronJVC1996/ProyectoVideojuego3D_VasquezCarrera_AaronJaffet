using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CamaraController : MonoBehaviour
{
    // Start is called before the first frame update   [Header("Camera Settings")] [Header("Cinemachine Settings")]
    public CinemachineFreeLook freeLookCamera;
    public Transform player; // El jugador a seguir

    public float rotationSpeed = 1f; // Velocidad de rotación de la cámara

    void Start()
    {
        // Asignar el objetivo de la cámara
        freeLookCamera.Follow = player;
        freeLookCamera.LookAt = player;
    }

    void Update()
    {
        // Rotar la cámara con las teclas de dirección o el ratón
        float horizontalInput = Input.GetAxis("Mouse X"); // Movimiento horizontal
        float verticalInput = Input.GetAxis("Mouse Y");   // Movimiento vertical

        // Aplicar la rotación de la cámara
        freeLookCamera.m_XAxis.Value += horizontalInput * rotationSpeed * Time.deltaTime;
        freeLookCamera.m_YAxis.Value -= verticalInput * rotationSpeed * Time.deltaTime;
    }
}