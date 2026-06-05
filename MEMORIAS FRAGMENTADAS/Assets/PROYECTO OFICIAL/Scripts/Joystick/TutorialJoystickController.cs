using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable CS0414
public class TutorialJoystickController : MonoBehaviour
{
    [Header("Escena base")]
    public string nombreEscenaBase = "BASE";
    int vecesY = 0;
    public GameObject botonJugar;    
    bool usoX = false;
    bool usoA = false;
    bool usoB = false;
    bool usoY = false;

    bool tutorialCompleto = false;
    [Header("Indicadores")]
    public GameObject indicadorMovimiento;
    public GameObject indicadorX;
    public GameObject indicadorA;
    public GameObject indicadorB;
    public GameObject indicadorY;

    [Header("Botón siguiente")]
    public GameObject botonSiguiente;

    [Header("Escena siguiente")]
    public string nombreEscenaSiguiente = "Escena_VideoIntro";
    

    void Start()
    {
        vecesY = 0;
        tutorialCompleto = false;

        if (botonSiguiente != null)
            botonSiguiente.SetActive(false);

        if (botonJugar != null)
            botonJugar.SetActive(false);

        if (PlayerPrefs.GetInt("VolvioDelVideo", 0) == 1)
        {
            tutorialCompleto = true;
            vecesY = 1;

            if (botonSiguiente != null)
                botonSiguiente.SetActive(true);

            if (botonJugar != null)
                botonJugar.SetActive(true);
        }
    }

    void Update()
    {
        DetectarMovimiento();
        DetectarBotones();
        DetectarContinuar();
        DetectarJugar();
    }

    void DetectarMovimiento()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool moviendo = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;

        indicadorMovimiento.SetActive(moviendo);
    }

    void DetectarBotones()
    {
        bool xPresionado =
            Input.GetKey(KeyCode.X) ||
            Input.GetKey(KeyCode.JoystickButton2);

        bool aPresionado =
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.JoystickButton10);

        bool bPresionado =
            Input.GetKey(KeyCode.B) ||
            Input.GetKey(KeyCode.JoystickButton5);

        bool yPresionado =
            Input.GetKey(KeyCode.Y) ||
            Input.GetKey(KeyCode.JoystickButton3);

        indicadorX.SetActive(xPresionado);
        indicadorA.SetActive(aPresionado);
        indicadorB.SetActive(bPresionado);
        indicadorY.SetActive(yPresionado);

        if (xPresionado) usoX = true;
        if (aPresionado) usoA = true;
        if (bPresionado) usoB = true;
        if (yPresionado) usoY = true;
    }

    void DetectarContinuar()
    {
        bool yPresionadoAhora =
            Input.GetKeyDown(KeyCode.Y) ||
            Input.GetKeyDown(KeyCode.JoystickButton3);

        if (!yPresionadoAhora)
            return;

        vecesY++;

        Debug.Log("Veces que oprimió Y: " + vecesY);

        if (vecesY == 1)
        {
            tutorialCompleto = true;

            if (botonSiguiente != null)
                botonSiguiente.SetActive(true);

            return;
        }

        if (vecesY >= 2)
        {
            SceneManager.LoadScene(nombreEscenaSiguiente);
        }
    }

    void DetectarJugar()
    {
        bool bPresionadoAhora =
            Input.GetKeyDown(KeyCode.B) ||
            Input.GetKeyDown(KeyCode.JoystickButton5);

        if (!bPresionadoAhora)
            return;

        if (botonJugar != null && botonJugar.activeSelf)
        {
            PlayerPrefs.SetInt("VolvioDelVideo", 0);
            PlayerPrefs.Save();

            SceneManager.LoadScene(nombreEscenaBase);
        }
    }
}