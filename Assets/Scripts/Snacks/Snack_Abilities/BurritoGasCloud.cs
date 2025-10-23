using UnityEngine;

public class BurritoGasCloud : SnackAbility
{
    #region Burrito Variables
    [Header("Gas Cloud")]
    [SerializeField] private GameObject _gasCloudPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private float _cloudRadius = 3.5f;
    [SerializeField] private float _duration = 6f;
    [SerializeField] private float _slowPercent = 0.5f;
    [SerializeField] private LayerMask _affectLayers;
    [SerializeField] private AudioClip sfx;
    [SerializeField] private float sfxVolume = 0.9f;

    #endregion

    protected override void PerformAbility()
    {
        Transform p = _spawnPoint != null ? _spawnPoint : transform;
        GameObject cloud = Instantiate(_gasCloudPrefab, p.position, Quaternion.identity);

        var gc = cloud.GetComponent<GasCloud>();
        if(gc == null)
        {
            gc = cloud.AddComponent<GasCloud>();

        }
        gc.Initialize(
            radius: _cloudRadius,
            duration: _duration,
            slowPercent: _slowPercent,
            affectLayers: _affectLayers

        );

        if(sfx)
        {
            AudioSource.PlayClipAtPoint(sfx, p.position, sfxVolume);
        }
    }


}
