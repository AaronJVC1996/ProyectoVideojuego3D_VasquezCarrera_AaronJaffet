using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;   // Velocidad de movimiento
    public float jumpForce = 7f;   // Fuerza de salto
    public float rotationSpeed = 700f;  // Velocidad de rotación del personaje

    [Header("References")]
    public Rigidbody rb;  // Referencia al Rigidbody del personaje
    public Transform cameraTransform;  // Referencia a la cámara para la dirección del movimiento

    private float moveX, moveZ;

    private bool isGrounded; // Para comprobar si el personaje está en el suelo

    void Start()
    {
        // Si no se asignó un Rigidbody en el Inspector, lo conseguimos automáticamente
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        // Configuramos la gravedad en el Rigidbody si no se asignó
        rb.useGravity = true;
    }

    void Update()
    {
        // Obtención de entradas para el movimiento (W, A, S, D o flechas)
        moveX = Input.GetAxis("Horizontal"); // Movimiento en el eje X (izquierda/derecha)
        moveZ = Input.GetAxis("Vertical");   // Movimiento en el eje Z (adelante/atrás)

        // Movimiento del personaje en 3D (usando la cámara para la dirección)
        Vector3 moveDirection = (cameraTransform.forward * moveZ) + (cameraTransform.right * moveX);
        moveDirection.y = 0f; // Descartamos cualquier movimiento en el eje Y para que no se mueva hacia arriba o abajo

        // Aplicamos el movimiento al Rigidbody en los ejes X y Z
        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);

        // Rotación suave hacia la dirección en la que se mueve el personaje
        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        // Comprobación de si el jugador está en el suelo antes de permitir saltar
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f); // Raycast hacia abajo para comprobar el suelo

        // Salto si el jugador está en el suelo y presiona la barra espaciadora
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    private void Jump()
    {
        // Aplicamos la fuerza para saltar
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}