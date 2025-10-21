using UnityEngine;

public class BossStatus : Status
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update() { }

    public override bool Hit(float damage, Vector2 knockback)
    {
        if(IsAlive)
        {
            float finalDamage = Mathf.Max(0, damage);
            CurrentHealth -= finalDamage;

            damageableHit?.Invoke(damage, knockback);
            CharacterEvents.characterDamaged?.Invoke(gameObject, damage);

            return true;
        }

        return false;
    }

    protected override void OnDied()
    {
        base.OnDied();
    }
}
