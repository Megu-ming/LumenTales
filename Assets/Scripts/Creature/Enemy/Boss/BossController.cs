using UnityEngine;
using UnityEngine.Events;

public class BossController : CreatureController
{
    public Transform player;

    private void Start()
    {
        if (player == null && Player.instance != null)
        {
            player = Player.instance.transform;
        }
    }

    public void LookAtPlayer()
    {
        if(transform.position.x > player.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }
}
