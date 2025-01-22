using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Camera : MonoBehaviour
{
    [SerializeField] private float accelerationMultipler;
	[SerializeField] private float cameraMoveSpeed;
    [SerializeField] private Rigidbody rb;
	private Vector3 startPos;
	private Vector3 displacement;
	private Vector3 velocity;
	private Vector3 lastVel;
	private Vector3 acceleration;
    
    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void LateUpdate()
    {
        Vector3 nextPos = startPos + (-acceleration * accelerationMultipler);
        displacement = Vector3.Lerp(displacement, nextPos, cameraMoveSpeed * Time.deltaTime);
        transform.localPosition = displacement;
    }

    private void FixedUpdate(){
        velocity = rb.transform.InverseTransformDirection(rb.velocity);
        acceleration = (velocity - lastVel) / Time.deltaTime;
        lastVel = velocity;
    }
}
