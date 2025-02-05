using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] GameObject cannonball;
    [SerializeField] float cannonballSpeed;

    GameObject player;
    Quaternion playerDirection;
    Rigidbody cannonRB;
    Timer cannonballSpawnTimer;
    float cannonballSpawnTime = 2f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        cannonRB = GetComponent<Rigidbody>();
        cannonballSpawnTimer = gameObject.AddComponent<Timer>();
        cannonballSpawnTimer.onTimerComplete += SpawnCannonball;
        cannonballSpawnTimer.StartTimer(cannonballSpawnTime);
    }

    // Update is called once per frame
    void Update()
    {
        playerDirection = Quaternion.LookRotation(player.transform.position - transform.position);
        playerDirection = Quaternion.Slerp(transform.rotation, playerDirection, 100 * Time.deltaTime);
        cannonRB.MoveRotation(playerDirection);
    }

    void SpawnCannonball()
    {
        Vector3 offset = new Vector3(1f, 0, 0);
        GameObject spawnedCannonball = Instantiate(cannonball, transform.position + offset, Quaternion.identity);
        Vector3 cannonballDirection = new Vector3(player.transform.position.x - spawnedCannonball.transform.position.x, 0, player.transform.position.z - spawnedCannonball.transform.position.z);
        spawnedCannonball.GetComponent<Rigidbody>().velocity = cannonballDirection * cannonballSpeed;
        cannonballSpawnTimer.StartTimer(cannonballSpawnTime);
    }
}
