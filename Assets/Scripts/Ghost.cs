using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float velocidad = 4f;
    public Mapa mapa;

    private Vector2 direccionActual;
    private Vector3 objetivo;

 
    void Start()
    {
        // Colocamos al fantasma en el centro de su celda inicial
        Vector3Int celda = mapa.tilemap.WorldToCell(transform.position);
        transform.position = mapa.tilemap.GetCellCenterWorld(celda);

        ElegirNuevaDireccion();
    }

    void Update()
    {
        MoverHaciaObjetivo();

        // Si llega al centro de la celda, elige otra dirección
        if (Vector3.Distance(transform.position, objetivo) < 0.01f)
        {
            SnapToGrid();
            ElegirNuevaDireccion();
        }
    }

    void MoverHaciaObjetivo()
    {
        transform.position = Vector3.MoveTowards(transform.position, objetivo, velocidad * Time.deltaTime);
    }

    void ElegirNuevaDireccion()
    {
        Vector3Int celda = mapa.tilemap.WorldToCell(transform.position);

        // Posibles direcciones
        Vector2[] dirs = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        // Elegimos una dirección válida al azar
        for (int i = 0; i < 10; i++)
        {
            Vector2 dir = dirs[Random.Range(0, dirs.Length)];
            Vector3Int siguiente = celda + new Vector3Int((int)dir.x, (int)dir.y, 0);

            if (!mapa.EsPared(siguiente))
            {
                direccionActual = dir;
                objetivo = mapa.tilemap.GetCellCenterWorld(siguiente);
                return;
            }
        }
    }

    void SnapToGrid()
    {
        Vector3Int celda = mapa.tilemap.WorldToCell(transform.position);
        transform.position = mapa.tilemap.GetCellCenterWorld(celda);
    }
}