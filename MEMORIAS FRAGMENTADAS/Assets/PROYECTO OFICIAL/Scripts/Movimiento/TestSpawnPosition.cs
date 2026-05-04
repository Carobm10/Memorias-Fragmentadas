using UnityEngine;

public class TestSpawnPosition : MonoBehaviour
{
    public Transform puntoSpawn;
    public Transform player;

    void Start()
    {
        if (puntoSpawn == null || player == null) return;

        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;
        }

        player.position = puntoSpawn.position;
        player.rotation = puntoSpawn.rotation;

        if (cc != null)
        {
            cc.enabled = true;
        }
    }
}