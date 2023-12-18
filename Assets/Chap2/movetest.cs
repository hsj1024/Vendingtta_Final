using UnityEngine;

public class movetest : MonoBehaviour
{
    public float moveSpeed = 50f;
    
    public Vector3 offset;
    public float yOffset;

    void Start()
    {
        
    }

    void Update()
    {
        transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);

    }
        
}
