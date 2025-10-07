using UnityEditor.Rendering.LookDev;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.InputSystem;

public class SnackController : MonoBehaviour
{
    #region Variables

    [Header("Snack Ability")]
    public SnackAbility currentAbility; //maybe make private later
    [SerializeField] float _abilityCooldown = 5f;
    private float _abilityTimer;

    [Header("Tool")]
    public SnackTool equippedTool; //maybe make private later

    //private StarterAssetsInputs _inputActions;

    #endregion

    void Awake()
    {
        //_inputActions = new PlayerInputActions();

        
    }

    // Update is called once per frame
    void Update()
    {
        HandleAbilityCooldown();

        if(Keyboard.current.qKey.wasPressedThisFrame)
        {
            UseTool();
        }

        if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            ActivateAbility();
        }

    }

    private void OnEnable()
    {
        //_inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        //_inputActions.Player.Disable();
    }

    #region Ability Methods
    private void ActivateAbility()
    {
        if (currentAbility != null && _abilityTimer <= 0)
        {
            currentAbility.Activate();
            _abilityTimer = _abilityCooldown;
        }
    }

    private void HandleAbilityCooldown()
    {
        if(_abilityTimer > 0)
        {
            _abilityTimer -= Time.deltaTime;
        }
    }

    private void UseTool()
    {
        if(equippedTool != null)
        {
            equippedTool.Use();
        }
    }

    #endregion


    #region Input System Binds
    

    #endregion
}
