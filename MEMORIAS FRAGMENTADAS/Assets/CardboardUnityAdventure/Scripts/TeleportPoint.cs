using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TeleportPoint : MonoBehaviour
{
    public UnityEvent OnTeleportEnter;
    public UnityEvent OnTeleport;
    public UnityEvent OnTeleportExit;

    void Start()
    {
        if (transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter()
    {
        OnTeleportEnter?.Invoke();
    }

    public void OnPointerClick()
    {
        ExecuteTeleportation();
        OnTeleport?.Invoke();
        //TeleportManager.Instance.DisableTeleportPoint(gameObject);
    }

    public void OnPointerExit()
    {
        OnTeleportExit?.Invoke();
    }

  //  private void ExecuteTeleportation()
   // {
      //  GameObject player = TeleportManager.Instance.Player;
//        player.transform.position = transform.position;

       // Camera camera = player.GetComponentInChildren<Camera>();

       // if (camera != null)
       // {
        //    float rotY = transform.rotation.eulerAngles.y - camera.transform.localEulerAngles.y;
          //  player.transform.rotation = Quaternion.Euler(0, rotY, 0);
      //  }
   // }

        private void ExecuteTeleportation()
    {
        GameObject player = TeleportManager.Instance.Player;

        Debug.Log("ANTES teleport Player: " + player.transform.position);

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.position = transform.position;

        Camera camera = player.GetComponentInChildren<Camera>();

        if (camera != null)
        {
            float rotY = transform.rotation.eulerAngles.y - camera.transform.localEulerAngles.y;
            player.transform.rotation = Quaternion.Euler(0, rotY, 0);
        }

        if (cc != null) cc.enabled = true;

        Debug.Log("DESPUÉS teleport Player: " + player.transform.position);
    }
}