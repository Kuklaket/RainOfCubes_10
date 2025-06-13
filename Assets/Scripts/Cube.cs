using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]

public class Cube : MonoBehaviour
{
    private bool _hasCollided = false;
    private Material _originalMaterial;

    public Renderer CubeRenderer { get; private set; }

    public event Action<GameObject> CubeReadyForRelease;

    private void Awake()
    {
        CubeRenderer = GetComponent<Renderer>();
        _originalMaterial = new Material(CubeRenderer.material);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_hasCollided && collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            _hasCollided = true; 
            CubeRenderer.material.color = Color.red;
            
            StartCoroutine(ReleaseAfterDelay(SetRandomTime()));
        }
    }

    public void ResetColor()
    {
        if (CubeRenderer != null && _originalMaterial != null)
        {
            CubeRenderer.material = _originalMaterial;
            _hasCollided = false;
        }
    }

    private IEnumerator ReleaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        CubeReadyForRelease?.Invoke(this.gameObject);
    }

    private float SetRandomTime()
    {
        int minCountTime = 2;
        int maxCountTime = 5;

        float lifeTimeCount = UnityEngine.Random.Range (minCountTime, maxCountTime + 1);

        return lifeTimeCount;
    }
}


