using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flight_Inputs : MonoBehaviour
{
    [HideInInspector] public float mouseXInput { get { return Input.GetAxis("Roll"); } private set {} }
    [HideInInspector] public float mouseYInput { get { return Input.GetAxis("Pitch"); } private set {} }
    [HideInInspector] public float horizontalInput { get { return Input.GetAxis("Yaw"); } private set {} }
     
    [HideInInspector] public Vector3 planeVectors { get => GetPlaneVectors(); private set {} }
    public Vector3 planeInputs;
    // Pitch Yaw Roll
    [SerializeField] private Vector3 planeVectorsSensitivity;
    // Camera
    public Vector2 minMaxFOV = new Vector2(40, 70);
    [SerializeField] private Camera playerCamera;
    public Animator animator;
    public float throttle;
    public Vector2 throttleTargets = new Vector2(1f, 0f); // W, Default
    public UI_Controller UIController;
    [SerializeField] private Vector2 throttleAccDecel = new Vector2(2f, 4f);
    private float currFOV;

    private void Start(){
        Wingsuit currentWingsuit = GameController.instance.wingsuits[GameController.instance.currentWingsuit];
        animator = (Animator) Instantiate(currentWingsuit.wingsuitPrefab, transform.position, transform.rotation);
        animator.transform.Rotate(0f, 180f, 0f);
        animator.transform.SetParent(this.transform);
        currFOV = minMaxFOV.x;
        playerCamera.fieldOfView = currFOV;
    }

    private void Update(){
        float throttleTarget = throttleTargets.y;
        float pitchAngle = transform.eulerAngles.x;
        float pitchAngleRadians = pitchAngle * Mathf.Deg2Rad;
        float thrustFromSine = Mathf.Sin(pitchAngleRadians);
        
        bool canAccelerate = (thrustFromSine > 0f);

        bool isDiving = false;
        if (Input.GetKey(KeyCode.W)){
            if (canAccelerate) throttleTarget = throttleTargets.x;
            isDiving = true;

        }
        animator.SetBool("isDiving", isDiving);
        float throttleLerpSpeed = canAccelerate ? throttleAccDecel.x : throttleAccDecel.y;
        throttle = Mathf.MoveTowards(throttle, throttleTarget, Time.deltaTime * throttleLerpSpeed); // This needs to be made sure its frame independent
        float FOVt = Mathf.InverseLerp(throttleTargets.x, throttleTargets.y, throttle);
        playerCamera.fieldOfView = Mathf.Lerp(minMaxFOV.x, minMaxFOV.y, 1-FOVt);
        planeInputs = planeVectors;

        if (Input.GetKeyDown(KeyCode.Escape)){
            UIController.TogglePauseMenu();
        }
    }

    private Vector3 GetPlaneVectors(){
        Vector3 t_planeVectors = Vector3.zero;
        t_planeVectors.x = mouseYInput * planeVectorsSensitivity.y;
        t_planeVectors.y = horizontalInput * planeVectorsSensitivity.z;
        t_planeVectors.z = mouseXInput * planeVectorsSensitivity.x;
 
        return t_planeVectors;
    }

}