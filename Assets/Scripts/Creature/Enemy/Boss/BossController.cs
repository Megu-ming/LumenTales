using UnityEngine;
using UnityEngine.Events;

public class BossController : CreatureController
{
    Transform player;
    public EnemyStatus status;

    private void Start()
    {
        if (player == null && Player.instance != null)
        {
            player = Player.instance.transform;
        }
        if(status is null)
            status = GetComponent<EnemyStatus>();
    }

    public void LookAtPlayer()
    {
        if(transform.position.x > player.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }
}
