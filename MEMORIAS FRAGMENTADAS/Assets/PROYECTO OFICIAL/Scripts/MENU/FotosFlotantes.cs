using System.Collections.Generic;
using UnityEngine;

public class FotosFlotantes : MonoBehaviour
{
    public GameObject[] prefabsFotos; // tus 16 fotos
    public int cantidad = 20;

    public Vector3 tamanoArea = new Vector3(0.9f, 0.5f, 0.59f); // tamaño del cubo
    public float alturaFlotacion = 0.01f;
    public float velocidadFlotacion = 1f;

    private List<GameObject> fotosInstanciadas = new List<GameObject>();
    private List<Vector3> posicionesIniciales = new List<Vector3>();

    void Start()
    {
        for (int i = 0; i < cantidad; i++)
        {
            // Elegir prefab aleatorio
            GameObject prefab = prefabsFotos[Random.Range(0, prefabsFotos.Length)];

            // Posición aleatoria dentro del cubo
            Vector3 posicion = transform.position + new Vector3(
                Random.Range(-tamanoArea.x / 2, tamanoArea.x / 2),
                Random.Range(-tamanoArea.y / 2, tamanoArea.y / 2),
                Random.Range(-tamanoArea.z / 2, tamanoArea.z / 2)
            );

            // Instanciar
            GameObject foto = Instantiate(prefab, posicion, Quaternion.identity);

            // Escala aleatoria
            float escala = Random.Range(0.5f, 1.1f);
            float escalaBase = 0.2f; // AJUSTA este valor
            foto.transform.localScale = Vector3.one * escala * escalaBase;

            fotosInstanciadas.Add(foto);
            posicionesIniciales.Add(foto.transform.position);
            foto.transform.rotation = Random.rotation;
        }
    }

    void Update()
    {
        for (int i = 0; i < fotosInstanciadas.Count; i++)
        {
            if (fotosInstanciadas[i] == null) continue;

            Vector3 posInicial = posicionesIniciales[i];

            float offset = Mathf.Sin(Time.time * velocidadFlotacion + i) * alturaFlotacion;

            fotosInstanciadas[i].transform.position = new Vector3(
                posInicial.x,
                posInicial.y + offset,
                posInicial.z
            );
            fotosInstanciadas[i].transform.Rotate(0, 10f * Time.deltaTime, 0);

            Debug.Log("Instanciando foto #" + i);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, tamanoArea);
    }
}