using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  // 👈 agregar esto

public class CardboardSimulator : MonoBehaviour
{
    public bool UseCardboardSimulator = true;
    [SerializeField] private float horizontalSpeed = 0.5f;
    [SerializeField] private float verticalSpeed = 0.5f;
    [SerializeField] private float rotationX = 0.0f;
    [SerializeField] private float rotationY = 0.0f;
    private Camera cam;

    void Start()
    {
#if UNITY_EDITOR
        cam = Camera.main;
#endif
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!UseCardboardSimulator)
            return;

        // 👇 Mouse nuevo sistema
        if (Mouse.current.leftButton.isPressed)
        {
            float mouseX = Mouse.current.delta.x.ReadValue() * horizontalSpeed * 0.1f;
            float mouseY = Mouse.current.delta.y.ReadValue() * verticalSpeed * 0.1f;

            rotationY += mouseX;
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -45, 45);
            cam.transform.localEulerAngles = new Vector3(rotationX, rotationY, 0.0f);
        }
#endif
    }

    public void UpdatePlayerPositonSimulator()
    {
        rotationX = 0;
        rotationY = cam.transform.localEulerAngles.y;
    }
}