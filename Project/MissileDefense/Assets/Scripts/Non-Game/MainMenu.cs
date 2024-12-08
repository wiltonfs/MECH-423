using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu: MonoBehaviour
{
    public TextMeshProUGUI selectText;
    private TextMeshProUGUI displayText;
    private SerialScanner SerialScanner;
    private string baseText = "";
    private int dotCount = 0;
    private bool threatTriggered = false;

    private float slowType = 0.03f;
    private float fastType = 0.01f;

    private long baseEncoderCounts;
    private uint baseFireCommands;
    private bool arcadeSelected = true;

    void Start()
    {
        displayText = GetComponent<TextMeshProUGUI>();
        baseText = displayText.text;
        displayText.text = ""; // Clear initial text
        SerialScanner = FindObjectOfType<SerialScanner>();
        StartCoroutine(TypeText(baseText)); // Start typing animation
        StartCoroutine(HandleSelection()); // Start logging inputs

        baseEncoderCounts = SerialScanner.CummulativeEncoderCounts;
        baseFireCommands = SerialScanner.RxdLaunchCommands;
    }

    void Update()
    {
        bool FirePressed = SerialScanner.RxdLaunchCommands > baseFireCommands;
        // Handle starting a level
        if (!threatTriggered)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || (arcadeSelected && (Input.GetKeyDown(KeyCode.Space) || FirePressed)))
            {
                threatTriggered = true;
                StopAllCoroutines();
                StartCoroutine(StartArcade());
            }

            if (Input.GetKeyDown(KeyCode.Alpha2) || (!arcadeSelected && (Input.GetKeyDown(KeyCode.Space) || FirePressed)))
            {
                threatTriggered = true;
                StopAllCoroutines();
                StartCoroutine(StartChallenge());
            }
        }
        
    }

    private IEnumerator HandleSelection()
    {
        while (true)
        {
            // If dial rotates, change the selection
            if (Mathf.Abs(SerialScanner.CummulativeEncoderCounts - baseEncoderCounts) > 2 || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                arcadeSelected = !arcadeSelected;
                baseEncoderCounts = SerialScanner.CummulativeEncoderCounts;
            }

            // Display selection
            if (arcadeSelected)
            {
                selectText.text = "-> Arcade Mode <-\t-- Challenge Mode --";
            }
            else
            {
                selectText.text = "-- Arcade Mode --\t-> Challenge Mode <-";
            }
            yield return null;
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
        StartCoroutine(AnimateDots()); // Start dots animation after typing finishes
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
    private IEnumerator StartArcade()
    {
        // Load modules
        string additionalText = "\n<color=orange>Threat detected.</color>\nStarting radar: ";
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
        yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
        additionalText = "<color=green>SUCCESS</color>\nWarming launcher lines: ";
        inTag = false; // Track if we're inside a tag
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
        yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
        additionalText = "<color=green>INITIATED</color>\nPrepare for control...";
        inTag = false; // Track if we're inside a tag
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

        // Start game
        SceneManager.LoadScene("ArcadeGame");
    }

    private IEnumerator StartChallenge()
    {
        // Load modules
        string additionalText = "\n<color=red>Critical threat detected.</color>\nStarting autopilot: ";
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
        yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
        additionalText = "<color=red>FAILED</color>\nArming warheads: ";
        inTag = false; // Track if we're inside a tag
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
        yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
        additionalText = "<color=green>INITIATED</color>\nManual control required\nPrepare for control...";
        inTag = false; // Track if we're inside a tag
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

        // Start game
        SceneManager.LoadScene("Game");
    }
}
