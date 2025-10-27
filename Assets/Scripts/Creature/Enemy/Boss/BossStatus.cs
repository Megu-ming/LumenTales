using UnityEngine;

public class BossStatus : Status
{
    public void Init()
    {
        CurrentHealth = BaseMaxHealth;
    }

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
