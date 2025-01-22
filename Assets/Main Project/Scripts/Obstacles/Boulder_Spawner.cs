using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder_Spawner : MonoBehaviour
{
    public void TriggerBouldersSpawn(){

    }

    [SerializeField] private bool canSpawn;
    [SerializeField] private float spawnTime = 3f;
    [SerializeField] private float torqueAmount = 5f;
    [SerializeField] private float forceAmount = 5f;

    [SerializeField] private List<Transform> spawnPoints = new List<Transform>(3);

    private void OnEnable()
    {
        //StartCoroutine(DisableText());
    }

    IEnumerator SpawnBoulders() {
        while (canSpawn) {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            GameObject boulder = Object_Pooler.Instance.SpawnFromPool("Boulders", spawnPoint.position, spawnPoint.rotation);
            Rigidbody boulderRb = boulder.GetComponent<Rigidbody>();
            boulderRb.AddTorque(boulder.transform.right * torqueAmount, ForceMode.Impulse);
            boulder.GetComponent<Rigidbody>().AddForce(spawnPoint.forward * forceAmount, ForceMode.Impulse);
            yield return new WaitForSeconds(spawnTime);
        }
    }

    private void OnTriggerEnter(Collider other){
        if (other.transform.tag == "Player"){
            Audio_Controller.PlaySound(SoundType.Boulders, 1f);
            StartCoroutine(SpawnBoulders());
            canSpawn = true;
        }
    }

    private void OnTriggerExit(Collider other){
        canSpawn = false;
    }
}
