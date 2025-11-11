using UnityEngine;

public class CreatureController : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator animator;

    public virtual void OnDead() { }
}