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
    private Renderer _renderer;

    public event UnityAction<Cube> LifetimeEnded;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void OnEnable()
    {        
        _renderer.material = _defaultMaterial;
    }

    private void OnDisable()
    {
        _hasBeenCollided = false;
    }

    private void Update()
    {
        if (transform.position.y < _minHeightForLiving)
            LifetimeEnded?.Invoke(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasBeenCollided == false)
        {
            if (collision.gameObject.GetComponent<Platform>())
            {
                _renderer.material = _triggeredMaterial;
                
                StartCoroutine(WaitLifetime(GetLifetime()));

                _hasBeenCollided = true;
            }
        }
    }

    private IEnumerator WaitLifetime(int lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        
        LifetimeEnded?.Invoke(this);
    }

    private int GetLifetime()
    {
        return Random.Range(_minLifetime, _maxLifetime + 1);
    }
}
