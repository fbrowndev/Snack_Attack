using UnityEngine;

public abstract class SnackTool : MonoBehaviour
{
    #region Variables
    [Header("Tool Settings")]
    [SerializeField] private string _toolName = "Tool";
    [SerializeField] private bool _isSingleUse = true;


    #endregion


    public void Use()
    {
        if (!CanUseTool())
        {
            Debug.Log($"{_toolName} is not usable right now.");
            return;
        }

        PerformToolEffect();

        if( _isSingleUse)
        {
            Destroy(gameObject);
        }
    }

    protected abstract void PerformToolEffect();

    protected virtual bool CanUseTool()
    {
        return true;
    }
}
