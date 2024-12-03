using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    private TextMeshProUGUI connectionText;
    private SerialScanner SerialScanner;
    private string baseText = "";
    private int dotCount = 0;

    void Start()
    {
        connectionText = GetComponent<TextMeshProUGUI>();
        baseText = connectionText.text;
        SerialScanner = FindObjectOfType<SerialScanner>();
        StartCoroutine(AnimateDots());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || (Time.timeSinceLevelLoad > 1f && SerialScanner.IsBoardConnected()))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private IEnumerator AnimateDots()
    {
        while (true)
        {
            dotCount = (dotCount + 1) % 4; // Cycle through 0, 1, 2, 3 dots
            connectionText.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(1.2f); // Adjust timing
        }
    }
}
