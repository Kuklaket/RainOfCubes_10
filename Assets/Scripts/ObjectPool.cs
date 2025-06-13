using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private int defaultCapacity = 5;
    [SerializeField] private int maxPoolSize = 5;
    [SerializeField] private float _repeatRate = 0.1f;

    private Vector3 _spawnAreaSize = new(5f, 0f, 5f);  
    private Cube _cubeComponent;
    private ObjectPool<GameObject> _cubePool; 


    private void Awake()
    {
        _cubePool = new ObjectPool<GameObject>(
            createFunc: CreateCube,
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) =>
            {
                obj.SetActive(false);
                obj.GetComponent<Cube>().ResetColor();
            },
            actionOnDestroy: OnDestroyCube, 
            collectionCheck: true, 
            defaultCapacity: defaultCapacity, 
            maxSize: maxPoolSize 
        );
    }

    private void Start()
    {
        InvokeRepeating(nameof(SetRandomPosition), 0.0f, _repeatRate);
    }

    private void SetRandomPosition()
    {
        GameObject cube = _cubePool.Get();

        cube.transform.position = transform.position + GetRandomPosition();   
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = new(
            Random.Range(-_spawnAreaSize.x / 2, _spawnAreaSize.x / 2),
            _spawnAreaSize.y,
            Random.Range(-_spawnAreaSize.z / 2, _spawnAreaSize.z / 2)
        );

        return randomPosition;
    }

    private GameObject CreateCube()
    {
        GameObject cube = Instantiate(cubePrefab);
        
        if (cube.TryGetComponent<Cube>(out _cubeComponent))
            _cubeComponent.CubeReadyForRelease += ReleaseCubeInPool;

        return cube;
    }

   private void ReleaseCubeInPool(GameObject cube)
    {
         _cubePool.Release(cube);       
    }

    private void OnDestroyCube(GameObject cube)
    {
        _cubeComponent.CubeReadyForRelease -= ReleaseCubeInPool;
        Destroy(cube);
    }
}