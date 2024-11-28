using UnityEngine;

public class EnemyController3D : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 10f; // Rango para detectar al jugador
    public float attackRange = 2f;    // Rango para iniciar ataque
    public LayerMask playerLayer;     // Capa del jugador
    private Transform player;         // Referencia al jugador detectado

    [Header("Movement")]
    public float moveSpeed = 3f;      // Velocidad de movimiento
    private bool isChasing = false;   // Indica si está persiguiendo al jugador

    [Header("Attack")]
    public float attackCooldown = 2f; // Tiempo entre ataques
    private float lastAttackTime = 0f;
    public Transform attackPoint;     // Punto desde donde ataca el enemigo
    public float attackRadius = 1f;   // Radio del ataque
    public int attackDamage = 10;     // Daño infligido por el ataque

    [Header("Components")]
    public Animator animator;         // Referencia al Animator

    private void Update()
    {
        DetectPlayer();
        HandleMovementAndAttack();
    }

    private void DetectPlayer()
    {
        // Detectar al jugador en el rango
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;
            isChasing = true;
        }
        else
        {
            player = null;
            isChasing = false;
            animator.SetBool("isMoving", false); // Cambiar a animación Idle
        }
    }

    private void HandleMovementAndAttack()
{
    if (player == null) return;

    float distanceToPlayer = Vector3.Distance(transform.position, player.position);

    if (distanceToPlayer > attackRange && isChasing)
    {
        // Mover hacia el jugador si no está atacando
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            animator.SetBool("isMoving", true);
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Rotar hacia el jugador
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
    else if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
    {
        // Iniciar ataque
        animator.SetBool("isMoving", false); // Detener animación de movimiento
        Attack();
    }
}

private void Attack()
{
    if (animator == null)
    {
        Debug.LogError("Animator no asignado.");
        return;
    }

    animator.SetTrigger("isAttacking");
    lastAttackTime = Time.time;

    if (attackPoint == null)
    {
        Debug.LogError("AttackPoint no asignado.");
        return;
    }

    Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius, playerLayer);

    foreach (Collider hit in hits)
    {
        if (hit.CompareTag("Player"))
        {
            Debug.Log("Jugador golpeado.");
            PlayerController playerController = hit.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(attackDamage);
            }
        }
    }
}

    private void OnDrawGizmosSelected()
    {
        // Dibujar rangos en la vista de escena
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}