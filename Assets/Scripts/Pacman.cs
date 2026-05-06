using System;
using UnityEngine;

public class PacmanMovimiento : MonoBehaviour
{
    public float velocidad = 6f;
    public Mapa mapa;

    // Dirección actual de movimiento
    private Vector2 direccionActual = Vector2.zero;

    // Dirección que el jugador quiere tomar
    private Vector2 direccionDeseada = Vector2.zero;

    // Margen para considerar que estamos en el centro de una celda
    public float margenCentro = 0.08f;

    void Start()
    {
        // Al iniciar, colocamos a Pac-Man EXACTAMENTE en el centro de su celda
        Vector3Int celda = mapa.tilemap.WorldToCell(transform.position);
        transform.position = mapa.tilemap.GetCellCenterWorld(celda);
    }

    void Update()
    {
        LeerInput();           // Guardamos la dirección deseada
        IntentarGiroCorrecto(); // Decidimos si podemos girar ahora o no
        Mover();                // Movemos a Pac-Man
    }

    // ---------------------------------------------------------
    // DETECCIÓN COLLISION FANTASMA
    // ---------------------------------------------------------
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Ghost"))
        {
            Destroy(gameObject);
            ScoreManager.Instance.scoreText.text = "Game Over";
        }
    }

    // ---------------------------------------------------------
    // LECTURA DE INPUT
    // ---------------------------------------------------------
    void LeerInput()
    {
        // Guardamos la dirección deseada, pero NO giramos aún
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            direccionDeseada = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direccionDeseada = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direccionDeseada = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direccionDeseada = Vector2.right;
        }
    }

    // ---------------------------------------------------------
    // CAMBIO DE ORIENTACIÓN SPRITE
    // ---------------------------------------------------------
    void RotarPacman() {

        if (direccionDeseada == Vector2.up)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (direccionDeseada == Vector2.down)
        {
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (direccionDeseada == Vector2.left)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (direccionDeseada == Vector2.right)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    // ---------------------------------------------------------
    // LÓGICA DE GIRO:
    // - MISMO EJE → giro inmediato
    // - EJES DISTINTOS → solo en el centro
    // ---------------------------------------------------------
    void IntentarGiroCorrecto()
    {
        // Si ya vamos en esa dirección, no hacemos nada
        if (direccionDeseada == direccionActual)
            return;

        // Obtenemos celda actual y su centro
        Vector3Int celda = mapa.tilemap.WorldToCell(transform.position);
        Vector3 centro = mapa.tilemap.GetCellCenterWorld(celda);

        // Distancia al centro (para saber si podemos girar)
        float distCentro = Vector3.Distance(transform.position, centro);

        // Celda hacia donde queremos girar
        Vector3Int celdaSiguiente = celda + new Vector3Int((int)direccionDeseada.x, (int)direccionDeseada.y, 0);

        // Si hay pared en esa dirección → no giramos
        if (mapa.EsPared(celdaSiguiente))
            return;

        // ¿Estamos girando dentro del mismo eje?
        bool mismoEje =
            (direccionActual.x != 0 && direccionDeseada.x != 0) || // izquierda ↔ derecha
            (direccionActual.y != 0 && direccionDeseada.y != 0);   // arriba ↔ abajo

        // -----------------------------
        // MISMO EJE → GIRO INMEDIATO
        // -----------------------------
        if (mismoEje)
        {
            direccionActual = direccionDeseada;
            RotarPacman();
            return;
        }

        // -----------------------------
        // EJES DISTINTOS → SOLO EN EL CENTRO
        // -----------------------------
        if (distCentro <= margenCentro)
        {
            // Ajustamos al centro para evitar vibraciones
            transform.position = centro;

            // Ahora sí cambiamos de dirección
            direccionActual = direccionDeseada;

            RotarPacman();
        }
    }

    // ---------------------------------------------------------
    // MOVIMIENTO Y AJUSTE AL CENTRO CUANDO HAY PARED
    // ---------------------------------------------------------
    void Mover()
    {
        // Si no tenemos dirección, no nos movemos
        if (direccionActual == Vector2.zero)
            return;

        Vector3 pos = transform.position;

        // Celda actual
        Vector3Int celda = mapa.tilemap.WorldToCell(pos);

        // Celda hacia donde nos movemos
        Vector3Int celdaFrente = celda + new Vector3Int((int)direccionActual.x, (int)direccionActual.y, 0);

        // Si hay pared delante → debemos detenernos en el centro
        if (mapa.EsPared(celdaFrente))
        {
            Vector3 centro = mapa.tilemap.GetCellCenterWorld(celda);

            // Distancia al centro
            float dist = Vector3.Distance(transform.position, centro);

            // Cuánto avanzamos este frame
            float paso = velocidad * Time.deltaTime;

            // Si en este frame ya llegamos al centro → lo clavamos EXACTO
            if (dist <= paso)
            {
                transform.position = centro;
                direccionActual = Vector2.zero; // Nos detenemos
            }
            else
            {
                // Si aún no llegamos, avanzamos hacia el centro
                transform.position = Vector3.MoveTowards(transform.position, centro, paso);
            }

            return;
        }

        // Si no hay pared, avanzamos normal
        transform.position += (Vector3)direccionActual * velocidad * Time.deltaTime;
    }
}