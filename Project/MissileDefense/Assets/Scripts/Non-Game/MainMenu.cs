using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu: MonoBehaviour
{
    private TextMeshProUGUI displayText;
    private SerialScanner SerialScanner;
    private string baseText = "";
    private int dotCount = 0;
    private bool threatTriggered = false;

    private float slowType = 0.06f;
    private float fastType = 0.03f;

    private Coroutine visualCoroutine; // Store reference to the visual coroutine so we can stop it when we detect a threat

    void Start()
    {
        displayText = GetComponent<TextMeshProUGUI>();
        baseText = displayText.text;
        displayText.text = ""; // Clear initial text
        SerialScanner = FindObjectOfType<SerialScanner>();
        visualCoroutine = StartCoroutine(TypeText(baseText)); // Start typing animation
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !threatTriggered)
        {
            threatTriggered = true;
            StartCoroutine(HandleThreatAndLoadGame());
        }
    }
    private IEnumerator TypeText(string text)
    {
        bool inTag = false; // Track if we're inside a tag
        foreach (char c in text)
        {
            if (c == '<') inTag = true;  // Detect the start of a tag
            if (!inTag)
            {
                displayText.text += c;
                yield return new WaitForSeconds(Random.Range(fastType, slowType)); // Add delay outside of tags
            }
            else
            {
                displayText.text += c; // Add tag characters immediately
            }
            if (c == '>') inTag = false; // Detect the end of a tag
        }
        visualCoroutine = StartCoroutine(AnimateDots()); // Start dots animation after typing finishes
    }

    private IEnumerator AnimateDots()
    {
        while (true)
        {
            dotCount = (dotCount + 1) % 4; // Cycle through 0, 1, 2, 3 dots
            displayText.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(1.2f); // Adjust timing
        }
    }
    private IEnumerator HandleThreatAndLoadGame()
    {
        if (visualCoroutine != null)
        {
            StopCoroutine(visualCoroutine); // Stop the current animation
        }
        // Load modules
        string additionalText = "\n<color=red>Threat detected.</color>\nStarting radar: ";
        bool inTag = false; // Track if we're inside a tag
        foreach (char c in additionalText)
        {
            if (c == '<') inTag = true;  // Detect the start of a tag
            if (!inTag)
            {
                displayText.text += c;
                yield return new WaitForSeconds(Random.Range(fastType, slowType)); // Add delay outside of tags
            }
            else
            {
                displayText.text += c; // Add tag characters immediately
            }
            if (c == '>') inTag = false; // Detect the end of a tag
        }
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        additionalText = "SUCCESS\nWarming launcher lines: ";
        foreach (char c in additionalText)
        {
            displayText.text += c;
            yield return new WaitForSeconds(Random.Range(fastType, slowType));
        }
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        additionalText = "INITIATED\nPrepare for control............";
        foreach (char c in additionalText)
        {
            displayText.text += c;
            yield return new WaitForSeconds(Random.Range(fastType, slowType));
        }

        // Start game
        SceneManager.LoadScene("Game");
    }
}
