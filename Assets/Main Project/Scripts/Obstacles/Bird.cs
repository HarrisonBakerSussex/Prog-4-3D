using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BirdState {
    Patrol,
    Chase,
    Attack
}
public class Bird : MonoBehaviour
{
    public Transform targetPoint, playerPoint, birdDropPoint;
    [SerializeField] private float moveSpeed, chaseMoveSpeed, rotateSpeed, chaseRotateSpeed, timeToTargetChange, birdHeight = 10, attackDistance = 100, attackRate, eggForce = 1000f, eggTorque= 100f;
    [SerializeField] private Vector2 minMaxRadiusPatrol, minMaxRadiusPlayer;
    [SerializeField] private Rigidbody rb;
    public float timeToStart;
    private Vector3 direction, velocity, target;
    public float timeSinceTargetChange, distanceFromTarget, previousTilt, movementDelay, fireTimer;
    private Quaternion rotateToTarget;
    [SerializeField] private BirdState currentState = BirdState.Patrol;

    private void OnEnable(){
        currentState = BirdState.Patrol;
        direction = Quaternion.Euler(rb.transform.eulerAngles) * Vector3.forward;
        movementDelay = timeToStart;
        if (timeToStart > 0f) rb.velocity = moveSpeed * direction;
    }

    private void Update(){
       switch (currentState){
            case BirdState.Patrol:
            break;
            case BirdState.Chase:
                CheckAttack();
            break;
            case BirdState.Attack:
                Attack();
                CheckAttack();
            break;
        }
    }

    private void CheckAttack(){
        if (distanceFromTarget <= attackDistance){
            currentState = BirdState.Attack;
        }
        else{
            currentState = BirdState.Chase;
        }
    }

    private void Attack(){  
        float timeDiff = Time.time - fireTimer; // Get time since last attack (in seconds)
        if (timeDiff > 60 / attackRate) // If time since last attack > (1 minute / attack rate in rounds per minute)
        {
            fireTimer = Time.time;
            // Attack
            GameObject egg = Object_Pooler.Instance.SpawnFromPool("Eggs", birdDropPoint.position, birdDropPoint.rotation);
            Rigidbody eggRb = egg.GetComponent<Rigidbody>();
            eggRb.velocity = rb.velocity;
            //eggRb.AddTorque(egg.transform.right * eggTorque, ForceMode.Impulse);
            eggRb.AddForce((Vector3.down * eggForce), ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other){
        if (other.transform.tag == "Player"){
            currentState = BirdState.Chase;
        }
    }

    private void OnTriggerExit(Collider other){
        if (other.transform.tag == "Player"){
            currentState = BirdState.Patrol;
        }
    }

    private void FixedUpdate(){
        // State machine stuff here
        switch (currentState){
            case BirdState.Patrol:
                MoveBird(targetPoint, minMaxRadiusPatrol, moveSpeed, rotateSpeed);
            break;
            case BirdState.Chase:
                MoveBird(playerPoint, minMaxRadiusPlayer, chaseMoveSpeed, chaseRotateSpeed);
            break;
            case BirdState.Attack:
                MoveBird(playerPoint, minMaxRadiusPlayer, chaseMoveSpeed, chaseRotateSpeed);
            break;
        }
    }

    private void MoveBird(Transform currentTarget, Vector2 minMaxRadius, float moveSpeed, float rotateSpeed){
        if (movementDelay > 0f){
            movementDelay-= Time.fixedDeltaTime;
            return;
        }
        // Get distance between bird and target
        Vector3 currentTargetPos = currentTarget.position;
        currentTargetPos.y = birdHeight;
        distanceFromTarget = Vector3.Magnitude(currentTargetPos - rb.position);
        // Get new target direction
        if (timeSinceTargetChange >= timeToTargetChange){
            target = GetRandomDirection(currentTargetPos, minMaxRadius);
            timeSinceTargetChange = 0;
        }
        timeSinceTargetChange += Time.fixedDeltaTime;

        target.y = 0;
        
        // Rotate Bird towards target
        if (target != Vector3.zero)
            rotateToTarget = Quaternion.LookRotation(target, Vector3.up);
        Vector3 targetRot = Quaternion.RotateTowards(rb.transform.rotation, rotateToTarget, rotateSpeed * Time.fixedDeltaTime).eulerAngles;
        targetRot.x = 0;
        rb.transform.eulerAngles = targetRot;

        // Calculate tilt of bird
        float tilt = Mathf.Clamp(Vector3.SignedAngle(target, direction, Vector3.up), -45f, 45f);
        float oldTilt = previousTilt;
        if (previousTilt < tilt)
            previousTilt += Mathf.Min(rotateSpeed * Time.fixedDeltaTime, tilt - previousTilt);
        else
            previousTilt -= Mathf.Min(rotateSpeed * Time.fixedDeltaTime, previousTilt - tilt);
        previousTilt = Mathf.Clamp(previousTilt, -45f, 45f);
        rb.transform.Rotate(0f, 0f, previousTilt - oldTilt);

        // Move bird
        direction = Quaternion.Euler(rb.transform.eulerAngles) * Vector3.forward;
        rb.velocity = moveSpeed * direction;
    }

    private Vector3 GetRandomDirection(Vector3 currentTargetPos, Vector2 minMaxRadius){
        Vector3 newDirection;
        if (distanceFromTarget > minMaxRadius.y)
        {
            newDirection = currentTargetPos - rb.transform.position;
        }
        else if (distanceFromTarget < minMaxRadius.x)
        {
            newDirection = rb.transform.position - currentTargetPos;
        } 
        else
        {
            // 360-degree freedom of choice on the horizontal plane
            float angleXZ = Random.Range(-Mathf.PI, Mathf.PI);
            // Calculate direction
            newDirection = Mathf.Sin(angleXZ) * Vector3.forward + Mathf.Cos(angleXZ) * Vector3.right;
        }
        return newDirection.normalized;
    }
}
