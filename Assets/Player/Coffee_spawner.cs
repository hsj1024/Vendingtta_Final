using UnityEngine;
using System.Collections.Generic;

public class Coffee_spawner : MonoBehaviour
{
    public GameObject coffeePrefab;
    public Transform coffeeSpawnPoint;
    public float coffeeSpeed = 300.0f;
    public int maxCoffeeCount = 10; // 최대 커피 생성 갯수
    private List<GameObject> coffees = new List<GameObject>();

    public void FireCoffee(Vector2 direction)
    {
        if (coffees.Count < maxCoffeeCount)
        {
            GameObject coffee = Instantiate(coffeePrefab, coffeeSpawnPoint.position, Quaternion.identity);
            Rigidbody2D rb = coffee.GetComponent<Rigidbody2D>();
            rb.velocity = direction.normalized * coffeeSpeed;
            Coffee coffeeScript = coffee.GetComponent<Coffee>();
            if (coffeeScript != null)
            {
                coffeeScript.launchDirection = direction.normalized;
            }

            coffees.Add(coffee); // 생성된 커피를 리스트에 추가
        }
    }

    private void Update()
    {
        for (int i = coffees.Count - 1; i >= 0; i--)
        {
            if (coffees[i] == null)
            {
                coffees.RemoveAt(i);
            }
        }
    }

    // 커피가 화면 밖으로 나가거나 소멸될 때 이 메서드를 호출하여 리스트에서 삭제
    public void RemoveCoffee(GameObject coffee)
    {
        if (coffees.Contains(coffee))
        {
            coffees.Remove(coffee);
            Destroy(coffee);
        }
    }
}
