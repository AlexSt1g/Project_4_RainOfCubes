using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Renderer))]
public class Cube : MonoBehaviour
{
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _triggeredMaterial;
    [SerializeField] private float _minHeightForLiving = -10f;    

    private readonly int _minLifetime = 2;
    private readonly int _maxLifetime = 5;    
    private bool _hasBeenCollided;
    private Coroutine _coroutine;

    public event UnityAction<Cube> LifetimeEnded;

    private void OnEnable()
    {
        GetComponent<Renderer>().material = _defaultMaterial;
    }

    private void OnDisable()
    {
        _hasBeenCollided = false;

        if (_coroutine != null)
            StopCoroutine(_coroutine);
    }

    private void Update()
    {
        if (transform.position.y < _minHeightForLiving)
            LifetimeEnded?.Invoke(GetComponent<Cube>());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasBeenCollided == false)
        {
            if (collision.gameObject.GetComponent<ColliderHolder>())
            {
                GetComponent<Renderer>().material = _triggeredMaterial;

                _coroutine = StartCoroutine(WaitLifetime(GetLifetime()));

                _hasBeenCollided = true;
            }
        }
    }

    private IEnumerator WaitLifetime(int lifetime)
    {
        yield return new WaitForSeconds(lifetime);

        LifetimeEnded?.Invoke(GetComponent<Cube>());
    }

    private int GetLifetime()
    {
        return Random.Range(_minLifetime, _maxLifetime + 1);
    }
}
