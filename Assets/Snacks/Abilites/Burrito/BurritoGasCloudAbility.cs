using UnityEngine;

[CreateAssetMenu(
    fileName = "BurritoGasCloud",
    menuName = "Snack Attack/Abilities/Burrito Gas Cloud"
    
    )]

public class BurritoGasCloudAbility : SnackAbilityBase
{

    public GameObject gasCloudPrefab;

    protected override void PerformAbility(GameObject owner)
    {
        Vector3 spawnPos = owner.transform.position;
        Instantiate(gasCloudPrefab, spawnPos, Quaternion.identity);
    }

}
