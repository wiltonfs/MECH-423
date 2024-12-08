using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI ScoreDisplayText;
    public GameObject GameOverVisualCollection;
    public TextMeshProUGUI GameOverScoreDisplayText;
    public TextMeshProUGUI InstructionsText;
    public int Score;
    // Start is called before the first frame update
    void Start()
    {
        RefreshScore();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RefreshScore()
    {
        ScoreDisplayText.text = $"Score: {Score}";
    }

    public void DestroyEnemyGetPoints(int points)
    {
        Score += points;
        RefreshScore();
    }

    public void ResetGame()
    { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }

    public void GameOver()
    {
        // Stop enemy spawning
        
        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        if (enemySpawner != null)
        {
            Destroy(enemySpawner.gameObject);
        }
        ArcadeSpawner arcade = FindObjectOfType<ArcadeSpawner>();
        if (arcade != null)
        {
            Destroy(arcade.gameObject);
        }

        // Clear enemies
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            enemy.DestroyEnemy();
        }
        // Stop missile shooting
        Destroy(FindObjectOfType<MissileLauncher>().gameObject);
        // Clear missiles
        Missile[] missiles = FindObjectsByType<Missile>(FindObjectsSortMode.None);
        foreach (Missile missile in missiles)
        {
            Destroy(missile.gameObject);
        }
        // Display Game Over window
        GameOverVisualCollection.gameObject.SetActive(true);
        GameOverScoreDisplayText.text = $"Final Score: {Score}";

        // Handle the game over logic
        StartCoroutine(HandleGameOverCoroutine());

        // Request the player's receipt to be printed
        SerialScanner serialScanner = FindObjectOfType<SerialScanner>();
        serialScanner.TransmitScoreToThermalPrinter(Score);
    }

    private IEnumerator HandleGameOverCoroutine()
    {
        SerialScanner serialScanner = FindObjectOfType<SerialScanner>();
        yield return new WaitForSeconds(0.5f);
        uint baseFireCommands = serialScanner.RxdLaunchCommands;
        long baseEncoder = serialScanner.CummulativeEncoderCounts;
        bool mainMenu = true;
        while (true)
        {
            // If dial rotates, change the selection
            if (Mathf.Abs(serialScanner.CummulativeEncoderCounts - baseEncoder) > 2 || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                mainMenu = !mainMenu;
                baseEncoder = serialScanner.CummulativeEncoderCounts;
            }

            // Display selection
            if (mainMenu)
            {
                InstructionsText.text = "-> Main Menu <-\n-- Play Again --";
            } else
            {
                InstructionsText.text = "-- Main Menu --\n-> Play Again <-";
            }

            // If fire button pressed, load selection
            if (serialScanner.RxdLaunchCommands > baseFireCommands || Input.GetKeyDown(KeyCode.Space))
            {
                if (mainMenu)
                {
                    SceneManager.LoadScene("MainMenu");
                } else
                {
                    SceneManager.LoadScene("ArcadeGame");
                }
            }
            yield return null;
        }
    }

}
