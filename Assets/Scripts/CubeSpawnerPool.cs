using UnityEngine;
using UnityEngine.Pool;

public class CubeSpawnerPool : MonoBehaviour
{
    [SerializeField] private GameObject _floor;
    [SerializeField] private float _startPointHeight = 50f;
    [SerializeField] private Cube _prefab;
    [SerializeField] private float _repeatRate = 0.2f;
    [SerializeField] private int _poolCapacity = 30;
    [SerializeField] private int _poolMaxSize = 30;        

    private Vector3 _startPoint;    
    private ObjectPool<Cube> _pool;         

    private void Awake()
    {                
        _startPoint = _floor.transform.position;
        _startPoint.y += _startPointHeight;

        _pool = new ObjectPool<Cube>(
        createFunc: () => Instantiate(_prefab),
        actionOnGet: (cube) => ActionOnGet(cube),
        actionOnRelease: (cube) => ActionOnRelease(cube),
        actionOnDestroy: (cube) => Destroy(cube),
        collectionCheck: true,
        defaultCapacity: _poolCapacity,
        maxSize: _poolMaxSize);
    }

    private void Start()
    {
        InvokeRepeating(nameof(GetCube), 0.0f, _repeatRate);
    }

    private void ActionOnGet(Cube cube)
    {
        SetSpawnPoint(cube);

        cube.gameObject.SetActive(true);   
        
        cube.LifetimeEnded += ReleaseCube;
    }

    private void SetSpawnPoint(Cube cube)
    {
        int halfOfScaleCoefficient = 2;
        int planeScaleCoefficient = 10;

        float sidestepX = _floor.transform.localScale.x / halfOfScaleCoefficient * planeScaleCoefficient;        
        float sidestepZ = _floor.transform.localScale.z / halfOfScaleCoefficient * planeScaleCoefficient;

        cube.transform.position = new Vector3(Random.Range(-sidestepX, sidestepX), _startPoint.y, Random.Range(-sidestepZ, sidestepZ));  
    }

    private void ActionOnRelease(Cube cube)
    {
        cube.gameObject.SetActive(false);        

        cube.LifetimeEnded -= ReleaseCube;
    }    

    private void GetCube()
    {
        _pool.Get();
    }

    private void ReleaseCube(Cube cube)
    {
        _pool.Release(cube);
    }    
}
