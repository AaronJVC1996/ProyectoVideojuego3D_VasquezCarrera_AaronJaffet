using Cinemachine;
using UnityEngine;

public class CamaraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float groundCheckDistance = 0.1f; // Distancia del raycast para verificar el suelo
    private bool isGrounded;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovePlayer();
        CheckGroundStatus();
    }

    void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0, vertical).normalized * moveSpeed;
        movement.y = rb.velocity.y; // Mantener la velocidad en el eje Y (salto y gravedad)
        rb.velocity = movement;
        
        // Saltar solo si está tocando el suelo
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void CheckGroundStatus()
    {
        // Realizamos un Raycast hacia abajo desde el centro del jugador para ver si está tocando el suelo
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance);

        // También puedes ajustar esto para detectar objetos específicos si lo necesitas.
    }

    void Jump()
    {
        // Salto
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
    }
}