using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 6f; // Velocidad al caminar
    public float runSpeed = 12f; // Velocidad al correr
    public float jumpForce = 7f; // Fuerza del salto
    public float gravityScale = 1f; // Escala de gravedad personalizada
    public LayerMask groundLayer; // Capa del suelo

    public Transform groundCheck; // Punto desde donde se verifica el suelo
    public float groundCheckRadius = 0.2f; // Radio para detectar el suelo

    private Rigidbody rb; // Referencia al Rigidbody
    private bool isGrounded; // Verifica si el jugador está tocando el suelo
    private float currentSpeed; // Velocidad actual según caminar o correr

    [Header("Animator")]
    public Animator animator; // Referencia al Animator

    [Header("Camera")]
    public Transform orientation; // Referencia a la orientación de la cámara

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("¡Falta un Rigidbody en el objeto!");
        }

        if (animator == null)
        {
            Debug.LogError("¡Falta el Animator en el objeto!");
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

        // Determinar la velocidad según si corre o camina
        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        if (moveDir != Vector3.zero)
        {
            rb.velocity = new Vector3(moveDir.x * currentSpeed, rb.velocity.y, moveDir.z * currentSpeed);

            // Configurar animación de caminar o correr
            animator.SetBool("IsWalking", !Input.GetKey(KeyCode.LeftShift));
            animator.SetBool("IsRunning", Input.GetKey(KeyCode.LeftShift));
        }
        else
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f); // Detener si no hay entrada

            // Configurar animación de idle
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }

        // Salto
        if (Input.GetButton("Jump") && isGrounded)
{
    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

    // Activar animación de salto inicial
    animator.SetTrigger("Jump");

    // Asegurarse de que no queden residuos del estado previo
    animator.ResetTrigger("Land");
    animator.SetBool("IsFalling", false);
}

        // Aplicar gravedad personalizada
        rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);

        // Configurar animación de caída y aterrizaje
if (!isGrounded && rb.velocity.y < 0)
{
    animator.SetBool("IsFalling", true); // Activar animación de caída
    animator.ResetTrigger("Land");      // Asegurar que no se active el aterrizaje
    animator.ResetTrigger("Jump");      // Prevenir que se reproduzca el salto inicial
}
else if (isGrounded && animator.GetBool("IsFalling"))
{
    animator.SetBool("IsFalling", false); // Detener la caída
    animator.SetTrigger("Land");          // Activar animación de aterrizaje
    animator.ResetTrigger("Jump");        // Reiniciar el Trigger del salto inicial
}
    }
}