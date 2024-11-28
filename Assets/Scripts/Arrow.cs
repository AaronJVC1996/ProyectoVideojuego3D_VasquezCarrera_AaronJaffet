using UnityEngine;

public class Arrow : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto tiene el tag "enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Destruir al enemigo
            Destroy(collision.gameObject);

            // Destruir la flecha
            Destroy(gameObject);
        }
        else
        {
            // Destruir la flecha si choca con otro objeto
            Destroy(gameObject);
        }
    }
}