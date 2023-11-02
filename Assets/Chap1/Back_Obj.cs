using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Back_Obj : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableObjects
    {
        public GameObject backPrefab;
        public string name;
        public List<GameObject> objects; 
        public List<Transform> spawnPoints; 
        public Vector2 xRange;
        public Vector2 yRange;
        public float xSpacing = 0f;
        public float ySpacing = 0f;
        public bool isFixed;
        public int minSpawnCount = 5; // �ּ� ������ ����
        public int maxSpawnCount = 10; // �ִ� ������ ����
    }


    [Header("Objects to Spawn")]
    public List<SpawnableObjects> spawnables; 

    [Header("Locker Spawners")]
    public List<GameObject> lockerPrefabs; // 4��¥�� 
    public List<GameObject> locker1Prefabs; // 1��¥�� 

    public float lockerSpacing = 10f;     // 4��¥�� ĳ����� X ��ǥ ����
    public float locker1Spacing = 5f;     // 1��¥�� ĳ����� X ��ǥ ����
    public float lockerYSpacing;

    public float lockerStartX = 10f;      // 4��¥�� ĳ��� ���� ����Ʈ
    public float locker1StartX = 20f;     // 1��¥�� ĳ��� ���� ����Ʈ
    public float mapEndX = 300f; // ���� �������� �Ÿ�
    private float lockerStartY = 1.45f;//y��ǥ ���� ��ġ
    public float locker1YSpacing;

    private void Start()
    {
        SpawnLockers();
        SpawnLocker1s();
        RandomizeSpawnPoints();

        AddObjectControllerToObjects();

        foreach (var spawnable in spawnables)
        {
            SpawnRandomObject(spawnable);
        }
    }

    private void RandomizeSpawnPoints()
    {
        foreach (var spawnable in spawnables)
        {
            if (!spawnable.isFixed)
            {
                for (int i = 0; i < spawnable.spawnPoints.Count; i++)
                {
                    SetRandomSpawnPointPosition(spawnable.spawnPoints[i], spawnable);
                }
            }
        }
    }

    void AddObjectControllerToObjects()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("obj");
        foreach (GameObject obj in objs)
        {
            if (obj.tag != "Cabinet")
            { // "Cabinet"�� ĳ��� ������Ʈ���� �±��Դϴ�.
                ObjectController objectController = obj.GetComponent<ObjectController>();
                if (objectController == null)
                {
                    obj.AddComponent<ObjectController>();
                }
            }
        }
    }


    private void SetRandomSpawnPointPosition(Transform point, SpawnableObjects spawnable)
    {
        if (point != null)
        {
            float randomX = Random.Range(spawnable.xRange.x, spawnable.xRange.y);
            float randomY = Random.Range(spawnable.yRange.x, spawnable.yRange.y);

            point.position = new Vector3(randomX, randomY, point.position.z);
        }
    }

    private void SpawnRandomObject(SpawnableObjects spawnable)
    {
        int numberOfSpawns = Random.Range(spawnable.minSpawnCount, spawnable.maxSpawnCount + 1);

        for (int i = 0; i < numberOfSpawns; i++)
        {
            if (spawnable.spawnPoints.Count == 0)
                break;

            int randomSpawnPointIndex = Random.Range(0, spawnable.spawnPoints.Count);
            Transform selectedSpawnPoint = spawnable.spawnPoints[randomSpawnPointIndex];

            int randomObjectIndex = Random.Range(0, spawnable.objects.Count);
            Instantiate(spawnable.objects[randomObjectIndex], selectedSpawnPoint.position, Quaternion.identity, transform);

            // ���õ� ���� ����Ʈ�� ��� ������ ���� ����Ʈ ��Ͽ��� �����մϴ�.
            spawnable.spawnPoints.RemoveAt(randomSpawnPointIndex);
        }
    }



    private void SpawnLockers()
    {
        float currentX = lockerStartX;
        float currentY = lockerStartY; // Y ��ǥ ���� ��ġ ����
        while (currentX < mapEndX)
        {
            int randomIndex = Random.Range(0, lockerPrefabs.Count);

            // Back_obj�� �θ�� �����Ͽ� ĳ��� ������Ʈ�� ����
            GameObject spawnedLocker = Instantiate(lockerPrefabs[randomIndex], new Vector3(currentX, currentY, 0), Quaternion.identity);
            spawnedLocker.transform.parent = transform; // �θ� ����

            currentX += lockerSpacing;
            currentY += lockerYSpacing; // Y ��ǥ ���� �߰�
        }
    }

    private void SpawnLocker1s()
    {
        float currentX = locker1StartX;
        float currentY = lockerStartY; // Y ��ǥ ���� ��ġ ����
        while (currentX < mapEndX)
        {
            int randomIndex = Random.Range(0, locker1Prefabs.Count);

            // Back_obj�� �θ�� �����Ͽ� ĳ��� ������Ʈ�� ����
            GameObject spawnedLocker1 = Instantiate(locker1Prefabs[randomIndex], new Vector3(currentX, currentY, 0), Quaternion.identity);
            spawnedLocker1.transform.parent = transform; // �θ� ����

            currentX += locker1Spacing;
            currentY += locker1YSpacing; // Y ��ǥ ���� �߰�
        }
    }


}