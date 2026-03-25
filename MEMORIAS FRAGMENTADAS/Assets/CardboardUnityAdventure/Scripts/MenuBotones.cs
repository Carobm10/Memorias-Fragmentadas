using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuBotones : MonoBehaviour
{
    public GameObject pantallaCarga;

    void Start()
    {
        if (pantallaCarga != null)
        {
            pantallaCarga.SetActive(false);
        }
    }

    void Update()
    {
        // X → Jugar
        if (Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            Jugar();
        }

        // A → Salir
        if (Input.GetKeyDown(KeyCode.JoystickButton11))
        {
            Salir();
        }
    }

    public void Jugar()
    {
        StartCoroutine(CargarEscena());
    }

    IEnumerator CargarEscena()
    {
        pantallaCarga.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene("CINEMATICA 1"); // usa tu nombre exacto
    }

    public void Salir()
    {
        Application.Quit();
    }
}