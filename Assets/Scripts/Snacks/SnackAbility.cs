using UnityEngine;

public abstract class SnackAbility : MonoBehaviour
{
    #region Variables
    [Header("Ability Settings")]
    [SerializeField] private float _cooldownDuration = 5f;
    [SerializeField] protected float cooldownTimer = 0f;

    [SerializeField] private bool _isOnCooldown => cooldownTimer > 0f;

    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public void Activate()
    {
        if(_isOnCooldown)
        {
            Debug.Log($"{gameObject.name} tried to use ability but cooldown still active");
            return;
        }

        PerformAbility();
        cooldownTimer = _cooldownDuration;
    }

    protected abstract void PerformAbility();
}
