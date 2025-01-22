using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UI_Controller : Base_Menu
{
    [SerializeField] private List<GameObject> menus = new List<GameObject>();
    [SerializeField] private GameObject winText, normalText, deadPlayer;
    [SerializeField] private TMP_Text distanceText, speedText, altitudeText, distanceResultText, pbDistanceText, coinsText, totalCoinsText;
    [SerializeField] private Transform startPos, player, camera, deathCamera;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private float coinsPerMetre = 1f;
    private bool isPaused = false;
    private int distance = 0;
    private bool canPause = true;

    private void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        OnStart();
        EnableUI("PlayerHud");
    }

    public void EnableUI(string UIName){
        EnableMenu(menus, UIName);
    }

    private void Update(){
        UpdateDistance();
        UpdateSpeed();
        UpdateAltitude();
    }

    private void OnTriggerEnter(Collider other){
        // If is player end game
        if (other.transform.tag == "Player"){
            normalText.SetActive(false);
            winText.SetActive(true);
            EndResult();
        }
    }

    public void TogglePauseMenu(){
        if(canPause){
            isPaused = !isPaused;
            if (isPaused) EnableUI("PauseMenu");
            else if (!isPaused) EnableUI("PlayerHud");
            Time.timeScale = isPaused ? 0 : 1;
            if (isPaused){
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else{
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    private void UpdateDistance(){
        distance = (int)Vector3.Distance(startPos.position, player.position);
        distanceText.text = "[ " + distance + "m" + " ]";
    }

    private void UpdateSpeed(){
        speedText.text = ((int)(playerRb.velocity.magnitude * 3.6)).ToString() + "km/h";
    }

    private void UpdateAltitude(){
        altitudeText.text = "↑ " + ((int)player.position.y).ToString() + "m";
    }

    public void EndResult(){
        // Need to disable player here
        deathCamera.position = camera.position;
        deathCamera.rotation = camera.rotation;
        deathCamera.gameObject.SetActive(true);
        Audio_Controller.PlaySound(SoundType.DeathSound, 1f);
        player.gameObject.SetActive(false);
        GameObject deadPlayerModel = Instantiate(deadPlayer, player.position, player.rotation);
        Rigidbody[] rbs = deadPlayer.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs){
            rb.AddExplosionForce(5f, deadPlayerModel.transform.position, 2f);
        }

        canPause = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Calculate coins based on distance travelled
        int coins =  (int)(distance * coinsPerMetre);
        int totalCoins = GameController.instance.PlayerCoins + coins;

        GameController.AddCoins(coins);
        // Update UI accordingly
        coinsText.text = "+£" + coins;
        totalCoinsText.text = "£" + totalCoins;
        distanceResultText.text = distance + "m";
        // get record distance
        pbDistanceText.text = GameController.instance.PreviousPb + "m";
        if (distance > GameController.instance.PreviousPb) GameController.SetNewPB(distance);
        // Load end game canvas
        EnableUI("ResultMenu");
    }
}
