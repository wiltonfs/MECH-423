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
        Destroy(FindObjectOfType<EnemySpawner>().gameObject);
        // Clear enemies
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            Destroy(enemy.gameObject);
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

        // Request the player's receipt to be printed
        SerialScanner serialScanner = FindObjectOfType<SerialScanner>();
        serialScanner.TransmitScoreToThermalPrinter(Score);
    }
}
