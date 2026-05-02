using System.Collections;
using TMPro;
using UnityEngine;

public class EfectoME : MonoBehaviour
{
    public float velocidad = 0.05f; // tiempo entre letras
    private TextMeshPro texto;
    private string textoCompleto;

    void Start()
    {
        texto = GetComponent<TextMeshPro>();
        textoCompleto = texto.text;
        texto.text = "";

        StartCoroutine(EscribirTexto());
    }

    IEnumerator EscribirTexto()
    {
        for (int i = 0; i <= textoCompleto.Length; i++)
        {
            texto.text = textoCompleto.Substring(0, i);
            yield return new WaitForSeconds(velocidad);
        }
    }
}
