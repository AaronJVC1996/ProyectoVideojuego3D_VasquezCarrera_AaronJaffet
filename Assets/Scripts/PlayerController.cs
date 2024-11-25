using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
     public float moveSpeed = 5f; // Velocidad de movimiento
    public float jumpForce = 5f; // Fuerza del salto
    public float gravityScale = 1f; // Escala de gravedad personalizada
    public LayerMask groundLayer; // Capa del suelo

    public Transform groundCheck; // Punto desde donde se verifica el suelo
    public float groundCheckRadius = 0.2f; // Radio para detectar el suelo

    private Rigidbody rb; // Referencia al Rigidbody
    private bool isGrounded; // Verifica si el jugador está tocando el suelo

    [Header("Camera")]
    public Transform orientation; // Referencia a la orientación de la cámara

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("¡Falta un Rigidbody en el objeto!");
        }

        // Congelar rotaciones no deseadas
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        // Verificar si está tocando el suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // Movimiento
        float moveInputVertical = Input.GetAxisRaw("Vertical"); // W/S o flechas arriba/abajo
        float moveInputHorizontal = Input.GetAxisRaw("Horizontal"); // A/D o flechas izquierda/derecha

        Vector3 moveDir = orientation.forward * moveInputVertical + orientation.right * moveInputHorizontal;
        moveDir.y = 0f; // Evitar movimiento en el eje Y
        moveDir.Normalize();

        if (moveDir != Vector3.zero)
        {
            rb.velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);
        }
        else
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f); // Detener si no hay entrada
        }

        // Salto
        if (Input.GetButton("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Aplicar gravedad personalizada
        rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);
    }
}