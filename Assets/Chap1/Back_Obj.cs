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
        public int minSpawnCount = 5; // 최소 스포너 갯수
        public int maxSpawnCount = 10; // 최대 스포너 갯수
    }


    [Header("Objects to Spawn")]
    public List<SpawnableObjects> spawnables; 

    [Header("Locker Spawners")]
    public List<GameObject> lockerPrefabs; // 4개짜리 
    public List<GameObject> locker1Prefabs; // 1개짜리 

    public float lockerSpacing = 10f;     // 4개짜리 캐비넷의 X 좌표 간격
    public float locker1Spacing = 5f;     // 1개짜리 캐비넷의 X 좌표 간격
    public float lockerYSpacing;

    public float lockerStartX = 10f;      // 4개짜리 캐비넷 시작 포인트
    public float locker1StartX = 20f;     // 1개짜리 캐비넷 시작 포인트
    public float mapEndX = 300f; // 맵의 끝까지의 거리
    private float lockerStartY = 1.45f;//y좌표 시작 위치
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
            { // "Cabinet"은 캐비넷 오브젝트들의 태그입니다.
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

            // 선택된 스폰 포인트를 사용 가능한 스폰 포인트 목록에서 제거합니다.
            spawnable.spawnPoints.RemoveAt(randomSpawnPointIndex);
        }
    }



    private void SpawnLockers()
    {
        float currentX = lockerStartX;
        float currentY = lockerStartY; // Y 좌표 시작 위치 설정
        while (currentX < mapEndX)
        {
            int randomIndex = Random.Range(0, lockerPrefabs.Count);

            // Back_obj를 부모로 설정하여 캐비넷 오브젝트를 생성
            GameObject spawnedLocker = Instantiate(lockerPrefabs[randomIndex], new Vector3(currentX, currentY, 0), Quaternion.identity);
            spawnedLocker.transform.parent = transform; // 부모 설정

            currentX += lockerSpacing;
            currentY += lockerYSpacing; // Y 좌표 간격 추가
        }
    }

    private void SpawnLocker1s()
    {
        float currentX = locker1StartX;
        float currentY = lockerStartY; // Y 좌표 시작 위치 설정
        while (currentX < mapEndX)
        {
            int randomIndex = Random.Range(0, locker1Prefabs.Count);

            // Back_obj를 부모로 설정하여 캐비넷 오브젝트를 생성
            GameObject spawnedLocker1 = Instantiate(locker1Prefabs[randomIndex], new Vector3(currentX, currentY, 0), Quaternion.identity);
            spawnedLocker1.transform.parent = transform; // 부모 설정

            currentX += locker1Spacing;
            currentY += locker1YSpacing; // Y 좌표 간격 추가
        }
    }


}