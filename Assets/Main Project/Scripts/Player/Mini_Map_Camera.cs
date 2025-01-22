using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mini_Map_Camera : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void Update(){
        Vector3 playerPos = player.position;
        playerPos.y = transform.position.y;
        transform.position = playerPos;
    }
}
