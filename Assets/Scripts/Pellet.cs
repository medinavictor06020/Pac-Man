using UnityEngine;
public class Pellet : MonoBehaviour
{
    public int valor = 10; // puntos que da este pellet

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo reaccionamos si el que entra es Pac-Man
        if (other.CompareTag("Player"))
        {
            ScoreManager.Instance.AddPoint(1);

            Destroy(gameObject); // El pellet desaparece

        }
    }
}