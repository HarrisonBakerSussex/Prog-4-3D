using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disable : MonoBehaviour
{
    [SerializeField] private float disableSpeed;

    private void OnEnable()
    {
        StartCoroutine(DisableText());
    }

    IEnumerator DisableText() {
        float timeElapsed = 0f;

        while (timeElapsed < disableSpeed) {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }

   
}
