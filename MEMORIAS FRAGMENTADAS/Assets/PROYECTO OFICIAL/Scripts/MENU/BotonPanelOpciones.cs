using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BotonPanelOpciones : MonoBehaviour
{
    [Header("Acción")]
    public string accion;

    [Header("Sprites")]
    public Sprite spriteNormal;
    public Sprite spritePresionado;

    [Header("Texto")]
    public TextMeshPro texto3D;

    public Color colorNormalTexto = Color.black;
    public Color colorPresionadoTexto = Color.white;

    private SpriteRenderer sr;
    private bool yaPresionado = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (sr != null && spriteNormal != null)
        {
            sr.sprite = spriteNormal;
        }
        if (texto3D != null)
        {
            texto3D.color = colorNormalTexto;
        }
    }

    public void Presionar()
    {
        if (yaPresionado)
            return;

        yaPresionado = true;

        // Cambiar sprite
        if (sr != null && spritePresionado != null)
        {
            sr.sprite = spritePresionado;
        }
        
        if (texto3D != null)
        {
            texto3D.color = colorPresionadoTexto;
        }

        // Acción
        if (accion == "JUGAR")
        {
            SceneManager.LoadScene("EscenaJuego");
        }
        else if (accion == "AJUSTES")
        {
            Debug.Log("Abrir ajustes");
        }
    }
}