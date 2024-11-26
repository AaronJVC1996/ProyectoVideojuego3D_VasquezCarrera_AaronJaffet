using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Para reiniciar la escena

public class WaterKill : MonoBehaviour
{
    [Header("Settings")]
    public string layerToKill = "WATER"; // Nombre del layer que mata
    public float playerDeathDelay = 1f; // Tiempo antes de que el jugador muera
    public string playerDeathAnimation = "Death_A"; // Nombre de la animación de muerte

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto pertenece al layer WATER
        if (gameObject.layer == LayerMask.NameToLayer(layerToKill))
        {
            // Si el objeto tiene el Tag "Player"
            if (other.CompareTag("Player"))
            {
                StartCoroutine(KillPlayer(other.gameObject));
            }
            // Si el objeto tiene el Tag "Enemy"
            else if (other.CompareTag("Enemy"))
            {
                KillEnemy(other.gameObject);
            }
        }
    }

    private IEnumerator KillPlayer(GameObject player)
    {
        Debug.Log("El jugador ha caído al agua. Morirá en breve.");

        // Activar la animación de muerte si tiene un Animator
        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(playerDeathAnimation);
        }

        // Esperar antes de reiniciar la escena
        yield return new WaitForSeconds(playerDeathDelay);

        // Reiniciar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void KillEnemy(GameObject enemy)
    {
        Debug.Log($"{enemy.name} ha muerto en el agua.");
        Destroy(enemy); // Destruir al enemigo inmediatamente
    }
}