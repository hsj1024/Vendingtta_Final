using UnityEngine;
using System.Collections.Generic;

public class Coffee_spawner : MonoBehaviour
{
    public GameObject coffeePrefab;
    public Transform coffeeSpawnPoint;
    public float coffeeSpeed = 300.0f;
    public int maxCoffeeCount = 10; // �ִ� Ŀ�� ���� ����
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

            coffees.Add(coffee); // ������ Ŀ�Ǹ� ����Ʈ�� �߰�
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

    // Ŀ�ǰ� ȭ�� ������ �����ų� �Ҹ�� �� �� �޼��带 ȣ���Ͽ� ����Ʈ���� ����
    public void RemoveCoffee(GameObject coffee)
    {
        if (coffees.Contains(coffee))
        {
            coffees.Remove(coffee);
            Destroy(coffee);
        }
    }
}
