using NUnit.Framework.Internal.Commands;
using System.Collections;
using UnityEngine;

[RequireComponent (typeof(SphereCollider))]
public class GasCloud : MonoBehaviour
{
    #region Gas Variables
    [Header("Runtime")]
    [SerializeField] private float _radius = 3.5f;
    [SerializeField] private float _duration = 6f;
    [SerializeField] private float _slowPercent = 0.5f;
    [SerializeField] private LayerMask _affectLayers;

    private SphereCollider _col;
    private ParticleSystem _ps;
    private Coroutine _lifeRoutine;
    private bool _initialized;

    #endregion

    private void Awake()
    {
        CacheComponent();
        ApplyColliderSettings();
    }

    private void OnEnable()
    {
        if(!_initialized)
        {
            StartCloud();
        }
    }

    public void Initialize(float radius, float duration,float slowPercent, LayerMask affectLayers)
    {
        _initialized = true;

        _radius = radius;
        _duration = duration;
        _slowPercent = slowPercent;
        _affectLayers = affectLayers;

        if(_col == null)
        {
            _col = GetComponent<SphereCollider>();
        }
        _col.isTrigger = true;
        _col.radius = _radius;

        if(_ps == null)
        {
            _ps = GetComponentInChildren<ParticleSystem>();
        }

        if(_ps)
        {
            var main = _ps.main;
            main.duration = _duration;
            _ps.Play();
        }

        StartCoroutine(LifeRoutine());
    }

    void CacheComponent()
    {
        if(_col == null)
        {
            _col = GetComponent<SphereCollider>();
        }

        if(_ps == null)
        {
            _ps = GetComponentInChildren<ParticleSystem>();
        }
    }

    void ApplyColliderSettings()
    {
        _col.isTrigger = true;
        _col.radius = _radius;
    }

    void StartCloud()
    {
        if(_lifeRoutine != null)
        {
            StopCoroutine(_lifeRoutine);
        }

        if(_ps)
        {
            var main = _ps.main;
            main.duration = _duration;
            _ps.Play();
        }

        _lifeRoutine = StartCoroutine(LifeRoutine());
    }



    IEnumerator LifeRoutine()
    {
        float t = 0f;
        while(t < _duration)
        {
            ApplyEffects();
            t += 0.2f; //tick rate
            yield return new WaitForSeconds(0.2f);
        }

        if(_ps)
        {
            _ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
         
        Destroy(gameObject, 0.75f);
    }

    void ApplyEffects()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _radius, _affectLayers, QueryTriggerInteraction.Collide);
        
        for(int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];

            //Add logic for temporary blinding of humans

            //Add logic to slow enemies
        }
    }

}
