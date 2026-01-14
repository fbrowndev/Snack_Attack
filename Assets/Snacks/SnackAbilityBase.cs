using UnityEngine;


public abstract class SnackAbilityBase : ScriptableObject
{
    [Header("Ability Settings")]
    [SerializeField] private float _cooldownDuration = 5f;

    protected float cooldownTimer;

    public bool IsOnCoolDown => cooldownTimer > 0f;

    public virtual void Tick(float deltaTime)
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= deltaTime;
        }
    }

    public void Activate(GameObject owner)
    {
        if(IsOnCoolDown)
        {
            Debug.Log($"{owner.name} tried to use ability but cooldown is still active");
            return;
        }

        PerformAbility(owner);
        cooldownTimer = _cooldownDuration;
    }

    protected abstract void PerformAbility(GameObject owner);
}
