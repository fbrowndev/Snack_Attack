using UnityEngine;

[CreateAssetMenu(
    fileName = "SnackData", 
    menuName = "Scriptable Objects/SnackData"
)]

public class SnackData : ScriptableObject
{
    //possibly change to private
    [Header("Snack Info")]
    public string snackName;
    [TextArea] public string snackDescription;

    [Header("Snack Ability")]
    public SnackAbilityBase ability;

    [Header("Snack Tool")] //Refine later?
    public SnackTool startingTool; //Attached tool? (Logic not worked out)

    [Header("Snack Visuals")] //Need for later
    public GameObject visualPrefab;
    public AnimatorOverrideController animatorOverride;

    [Header("Moblility Overrides")] //Need for later (Possible adjustments to movement stats of different players)
    public float moveSpeedMultiplier = 1f;
    public float speedMultiplier = 1f;
    public float jumpForceMultiplier = 1f;

    [Header("UI")]
    public Sprite icon;


    //Add skins here later
}
