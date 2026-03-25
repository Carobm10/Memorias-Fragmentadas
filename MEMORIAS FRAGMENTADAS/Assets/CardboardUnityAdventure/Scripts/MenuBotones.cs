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

    public void Jugar()
    {
        StartCoroutine(CargarEscena());
    }

    IEnumerator CargarEscena()
    {
        pantallaCarga.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene("CINEMATICA 1");
    }

    public void Salir()
    {
        Application.Quit();
    }
}