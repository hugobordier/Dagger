using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private GameObject groundPrefab;
    public float groundWidth = 20f; // largeur du prefab
    private List<GameObject> grounds = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = -1; i <= 2; i++)
        {
            GameObject g = Instantiate(groundPrefab, new Vector3(i * groundWidth, -1.74f, 0), Quaternion.identity);
            grounds.Add(g);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float playerX = player.position.x;
        GameObject leftMost = grounds[0];
        GameObject rightMost = grounds[grounds.Count - 1];

        if (playerX > rightMost.transform.position.x - groundWidth)
        {
            // Spawn nouveau à droite
            GameObject g = Instantiate(groundPrefab, rightMost.transform.position + Vector3.right * groundWidth, Quaternion.identity);
            grounds.Add(g);

            // Supprime celui de gauche
            Destroy(leftMost);
            grounds.RemoveAt(0);
        }
        
        if (playerX < leftMost.transform.position.x + groundWidth/2)
        {
            GameObject g = Instantiate(groundPrefab, leftMost.transform.position - Vector3.right * groundWidth, Quaternion.identity);
            grounds.Insert(0, g);

            Destroy(rightMost);
            grounds.RemoveAt(grounds.Count - 1);
        }
    }
}
