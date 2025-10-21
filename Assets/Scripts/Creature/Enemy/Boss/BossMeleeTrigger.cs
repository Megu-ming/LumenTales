using UnityEngine;

public class BossMeleeTrigger : MonoBehaviour
{
    [SerializeField] Collider2D meleeCollider;

    public void ActiveMelee()
    {
        meleeCollider.enabled = true;
    }

    public void DeactiveMelee()
    {
        meleeCollider.enabled = false;
    }

    
}
