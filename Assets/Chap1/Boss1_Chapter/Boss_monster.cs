using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_monster : MonoBehaviour
{
    public GameObject[] schoolStudentMalePrefabs;
    public GameObject[] schoolStudentFemalePrefabs;
    public Transform player;
    public float spawnInterval = 2f;
    public float spawnRateAcceleration = 0.05f;

    [Header("Speed Settings")]
    public float minSpeed = 1.5f;
    public float maxSpeed = 2.5f;

    [Header("Spawn Limit Settings")]
    public int maxMobCount = 10;
    public Camera mainCamera;

    [Header("Wave Settings")]
    public int monstersPerWave = 5;
    public float waveInterval = 180f;

    private List<Mob> mobs = new List<Mob>();

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // 게임 시작시 몇 마리의 몬스터를 미리 스폰
        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPosition = new Vector3(15, -2, 0);
            SpawnSingleMonster(spawnPosition);
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
                yield return new WaitForSeconds(spawnInterval);

                for (int i = 0; i < monstersPerWave && mobsInView < maxMobCount; i++)
                {
                    float playerX = player.position.x;
                    float minSpawnX = playerX - 14;
                    float maxSpawnX = playerX + 14;

                    float spawnX = Random.Range(minSpawnX - 10, maxSpawnX + 10);
                    while (Mathf.Abs(spawnX - playerX) < 5)
                    {
                        spawnX = Random.Range(minSpawnX - 10, maxSpawnX + 10);
                    }

                    Vector3 spawnPosition = new Vector3(spawnX, 0, 0);
                    SpawnSingleMonster(spawnPosition);
                    mobsInView++;
                }

                yield return new WaitForSeconds(waveInterval);
            }
            else
            {
                yield return null;
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
                if (screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1)
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

            monster.OnDeath += () =>
            {
                mobs.Remove(monster);
                Destroy(monsterGO, 10f);
            };
        }
    }
}
