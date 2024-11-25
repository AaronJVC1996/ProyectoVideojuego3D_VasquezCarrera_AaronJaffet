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

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Obtiene el Rigidbody del objeto
        if (rb == null)
        {
            Debug.LogError("¡Falta un Rigidbody en el objeto!");
        }
    }

    void Update()
    {
        // Verificar si está tocando el suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // Movimiento horizontal y vertical (WASD)
        float moveX = Input.GetAxis("Horizontal") * moveSpeed;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed;

        // Aplicar movimiento
        Vector3 movement = new Vector3(moveX, rb.velocity.y, moveZ);
        rb.velocity = movement;

        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Aplicar gravedad personalizada
        rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);
    }

    void OnDrawGizmosSelected()
    {
        // Dibuja un gizmo para visualizar el área de detección del suelo
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

// tenemos que poner A y D para rotar en vez de moverse