using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public int vida = 5;

    [Header("Movement")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpForce = 16f;
    public float gravityScale = 3f;
    public LayerMask groundLayer;

    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;

    private Rigidbody rb;
    private bool isGrounded;
    private float currentSpeed;

    [Header("Animator")]
    public Animator animator;

    [Header("Camera")]
    public Transform orientation;

    [Header("AIM")]
    public GameObject aimPoint;
    private bool isAiming = false;

    [Header("Arrow Shooting")]
    public GameObject arrowPrefab; // Prefab de la flecha
    public Transform shootPoint;  // Punto desde donde se dispara
    public float arrowSpeed = 20f; // Velocidad de la flecha

    private bool isDead = false;

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

        if (aimPoint == null)
        {
            Debug.LogError("¡Falta asignar el punto de AIM en el Canvas!");
        }
        else
        {
            aimPoint.SetActive(false);
        }

        if (arrowPrefab == null || shootPoint == null)
        {
            Debug.LogError("¡Falta asignar el prefab de la flecha o el punto de disparo!");
        }

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        HandleMovement();
        HandleAiming();

        if (isAiming)
        {
            HandleShooting();
        }
    }

    private void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        float moveInputVertical = Input.GetAxisRaw("Vertical");
        float moveInputHorizontal = Input.GetAxisRaw("Horizontal");

        Vector3 moveDir = orientation.forward * moveInputVertical + orientation.right * moveInputHorizontal;
        moveDir.y = 0f;
        moveDir.Normalize();

        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        if (moveDir != Vector3.zero)
        {
            rb.velocity = new Vector3(moveDir.x * currentSpeed, rb.velocity.y, moveDir.z * currentSpeed);

            animator.SetBool("IsWalking", !Input.GetKey(KeyCode.LeftShift));
            animator.SetBool("IsRunning", Input.GetKey(KeyCode.LeftShift));

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        else
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);

            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }

        if (Input.GetButton("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
            animator.ResetTrigger("Land");
            animator.SetBool("IsFalling", false);
        }

        rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);

        if (!isGrounded && rb.velocity.y < 0)
        {
            animator.SetBool("IsFalling", true);
            animator.ResetTrigger("Land");
            animator.ResetTrigger("Jump");
        }
        else if (isGrounded && animator.GetBool("IsFalling"))
        {
            animator.SetBool("IsFalling", false);
            animator.SetTrigger("Land");
            animator.ResetTrigger("Jump");
        }
    }

    private void HandleAiming()
    {
        if (Input.GetMouseButton(1)) // Click derecho
        {
            if (!isAiming)
            {
                isAiming = true;
                aimPoint.SetActive(true);
                animator.SetBool("IsAiming", true);
            }
        }
        else
        {
            if (isAiming)
            {
                isAiming = false;
                aimPoint.SetActive(false);
                animator.SetBool("IsAiming", false);
            }
        }
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0)) // Click izquierdo
        {
            ShootArrow();
        }
    }

    private void ShootArrow()
{
    // Instanciar la flecha en el punto de disparo
    GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.Euler(270f, shootPoint.eulerAngles.y, shootPoint.eulerAngles.z));

    // Obtener el Rigidbody de la flecha y aplicarle una fuerza
    Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();
    if (arrowRb != null)
    {
        arrowRb.velocity = shootPoint.forward * arrowSpeed;
    }

    // Destruir la flecha después de 3 segundos
    Destroy(arrow, 3f);
}

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        vida -= damage;

        if (vida > 0)
        {
            animator.SetTrigger("TakeHit");
        }
        else if (vida <= 0)
        {
            animator.SetTrigger("Death_A");
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        Invoke("RestartScene", 2f);
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}