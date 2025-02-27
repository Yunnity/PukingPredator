using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] GameObject cannonball;
    [SerializeField] float cannonballSpeed;

    Rigidbody cannonRB;
    Timer cannonballSpawnTimer;
    float cannonballSpawnTime = 2f;
    GameObject cannonMouth;

    // Start is called before the first frame update
    void Start()
    {
        cannonMouth = gameObject.transform.GetChild(0).gameObject;
        cannonballSpawnTimer = gameObject.AddComponent<Timer>();
        cannonballSpawnTimer.onTimerComplete += SpawnCannonball;
        cannonballSpawnTimer.StartTimer(cannonballSpawnTime);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void SpawnCannonball()
    {
        //Vector3 offset = new Vector3(0.2f, 0, 0);
        Vector3 offset = cannonMouth.transform.position - gameObject.transform.position;
        //Debug.Log($"cannonMouth.transform.position = {cannonMouth.transform.position}");
        GameObject spawnedCannonball = Instantiate(cannonball, cannonMouth.transform.position + offset, Quaternion.identity);
        //Debug.Log($"spawnedCannonball.transform.position.x = {spawnedCannonball.transform.position.x}");
        Vector3 cannonballDirection = new Vector3(spawnedCannonball.transform.position.x - gameObject.transform.position.x, 0, spawnedCannonball.transform.position.z - gameObject.transform.position.z);
        spawnedCannonball.GetComponent<Rigidbody>().velocity = cannonballDirection * cannonballSpeed;
        cannonballSpawnTimer.StartTimer(cannonballSpawnTime);
    }
}
