using UnityEngine;

public class PuertaAnimada : MonoBehaviour
{
    private Animator animator;
    private bool acostado = true; // empieza acostado

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("movement", 1f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            acostado = !acostado;
            animator.SetFloat("movement", acostado ? 1f : -1f);
        }
    }
}