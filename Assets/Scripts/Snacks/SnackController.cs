using System.Data;
using UnityEditor.Rendering.LookDev;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.InputSystem;

public class SnackController : MonoBehaviour
{
    #region Variables

    [Header("Loadout")]
    [SerializeField] private SnackData _snackData;

    [Header("Snack Attachments")]
    [SerializeField] private SnackAbilityBase _currentAbility;
    [SerializeField] private SnackTool _equippedTool;



    #endregion

    private void Awake()
    {
        ApplySnackData(_snackData);
    }



    // Update is called once per frame
    void Update()
    {
        //Tick Ability Cooldown
        if(_currentAbility != null)
        {
            _currentAbility.Tick(Time.deltaTime);
        }

        //Tool input
        if(Keyboard.current.qKey.wasPressedThisFrame)
        {
            UseTool();
        }

        //Ability Input
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ActivateAbility();
            
        }

    }

    public void ApplySnackData(SnackData data)
    {
<<<<<<< Updated upstream
        //_inputActions.Player.Enable();
=======
        _snackData = data;

        if(_snackData == null)
        {
            //Debug.LogWarning($"SnackController: No SnackData assigned '{_snackData.snackName}'");
            _currentAbility = null;
            _equippedTool = null;
            return;
        }

        _currentAbility = _snackData.ability != null ? Instantiate(_snackData.ability) : null;
        _equippedTool = _snackData.startingTool;

        Debug.Log($"SnackController: Applied SnackData '{_snackData.snackName}'");
>>>>>>> Stashed changes
    }

        _currentAbility = _snackData.ability != null ? Instantiate(_snackData.ability) : null;
        _equippedTool = _snackData.startingTool;

        Debug.Log($"SnackController: Applied SnackData '{_snackData.snackName}'");
    }


    #region Ability Methods
    private void ActivateAbility()
    {
        if(_currentAbility == null)
        {
            Debug.LogWarning("No ability equipped");
            return;
        }

        _currentAbility.Activate(gameObject);
    }

    private void UseTool()
    {
        if(_equippedTool != null)
        {
            _equippedTool.Use();
        }
    }

    #endregion
}
