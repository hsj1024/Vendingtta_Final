using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob_spawner : MonoBehaviour
{
    public GameObject[] schoolStudentMalePrefabs; // ���л� ���� ������ �迭
    public GameObject[] schoolStudentFemalePrefabs; // ���л� ���� ������ �迭
    public Transform player;
    public float spawnInterval = 5f;
    public float spawnRateAcceleration = 0.05f;

    [Header("Speed Settings")]
    public int minSpeed = 1;  // �ּ� �ӵ��� 1�� ����
    public int maxSpeed = 3;  // �ִ� �ӵ��� 3���� ����

    [Header("Spawn Limit Settings")]
    public int maxMobCount = 7;
    public Camera mainCamera;

    [Header("Wave Settings")]
    public int monstersPerWave = 5;
    public float waveInterval = 180f; // ���̺� ������ 180�ʷ� ����

    private List<Mob> mobs = new List<Mob>();

    void Start()
    {
        List<GameObject> malePrefabsList = new List<GameObject>(schoolStudentMalePrefabs);
        //malePrefabsList.Add((GameObject)Resources.Load("Mob2/Shcool_M2")); // 'Shcool_M2' ������ �ε� �� �߰�
        schoolStudentMalePrefabs = malePrefabsList.ToArray();

        List<GameObject> femalePrefabsList = new List<GameObject>(schoolStudentFemalePrefabs);
        //femalePrefabsList.Add((GameObject)Resources.Load("Mob2/Shcool_F2")); // 'Shcool_F2' ������ �ε� �� �߰�
        //femalePrefabsList.Add((GameObject)Resources.Load("Mob2/Shcool_F3")); // 'Shcool_F3' ������ �ε� �� �߰�
        schoolStudentFemalePrefabs = femalePrefabsList.ToArray();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        StartCoroutine(SpawnMonster());


    }

    IEnumerator SpawnMonster()
    {
        while (true)
        {
            int mobsInView = CountMobsInView();

            if (mobsInView < maxMobCount)
            {
                for (int i = 0; i < monstersPerWave && mobsInView < maxMobCount; i++)
                {
                    float playerX = player.position.x;
                    float halfCamWidth = mainCamera.orthographicSize * mainCamera.aspect;

                    float minSpawnX = playerX - halfCamWidth - 20;
                    float maxSpawnX = playerX + halfCamWidth + 20;

                    float spawnX = Random.Range(minSpawnX, maxSpawnX);
                    while (Mathf.Abs(spawnX - playerX) < 10)
                    {
                        spawnX = Random.Range(minSpawnX, maxSpawnX);
                    }

                    Vector3 spawnPosition = new Vector3(spawnX, 0, 0);
                    SpawnSingleMonster(spawnPosition);
                    mobsInView++;
                }

                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                yield return new WaitForSeconds(waveInterval);
            }
        }
    }

    int CountMobsInView()
    {
        int mobsInView = 0;
        foreach (Mob mob in mobs)
        {
            if (mob != null)
            {
                Vector3 screenPos = mainCamera.WorldToViewportPoint(mob.transform.position);
                if (screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= -1 && screenPos.y <= 1)
                {
                    mobsInView++;
                }
            }
        }
        return mobsInView;
    }

    void SpawnSingleMonster(Vector3 spawnPosition)
    {
        bool isMale = (Random.value > 0.5f);

        GameObject monsterGO;
        if (isMale)
        {
            int randomIndex = Random.Range(0, schoolStudentMalePrefabs.Length);
            monsterGO = Instantiate(schoolStudentMalePrefabs[randomIndex], spawnPosition, Quaternion.identity);
        }
        else
        {
            int randomIndex = Random.Range(0, schoolStudentFemalePrefabs.Length);
            monsterGO = Instantiate(schoolStudentFemalePrefabs[randomIndex], spawnPosition, Quaternion.identity);
        }

        monsterGO.transform.SetParent(this.transform);

        Mob monster = monsterGO.GetComponent<Mob>();
        if (monster != null)
        {
            monster.player = player;
            mobs.Add(monster);
            monster.moveSpeed = Random.Range(minSpeed, maxSpeed);
        }
    }
}
