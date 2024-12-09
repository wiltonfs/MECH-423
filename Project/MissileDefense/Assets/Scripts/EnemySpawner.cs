using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public EnemyData[] Enemies_LevelOne;
    public EnemyData[] Enemies_LevelTwo;
    public EnemyData[] Enemies_LevelThree;
    public TextMeshProUGUI EnemyDisplayText;
    public TextMeshProUGUI WaveDisplay;
    public TextMeshProUGUI TimerDisplay;
    public ushort[] waveMissileLevels;

    public bool waitingForPlayerToShootMissile;


    private MissileLauncher MissileLauncher;
    private SerialScanner SerialScanner;
    public EnemyData CurrentEnemy = null;

    private float startTime;
    // Start is called before the first frame update
    void Start()
    {
        MissileLauncher = FindObjectOfType<MissileLauncher>();
        SerialScanner = FindObjectOfType<SerialScanner>();

        startTime = Time.time;
        WaveDisplay.text = $"Wave 1/{waveMissileLevels.Length}";

        StartCoroutine(ChallengeSpawning());
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time - startTime;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        TimerDisplay.text = $"{minutes:00}:{seconds:00}";
    }

    private float RandomParam(uint Param, uint STD)
    {
        return IntRange((int)(Param-STD), (int)(Param +STD))/1000f;
    }

    private void UpdateEnemyDisplayedText()
    {
        EnemyDisplayText.text = string.Empty;
        if (CurrentEnemy == null) {
            return;
        }

        EnemyDisplayText.text += "Incoming Missile\n";
        EnemyDisplayText.text += $"RCS: {RandomParam(CurrentEnemy.RadarCrossSection, CurrentEnemy.RadarCrossSectionSTD):F3} m2\n";
        EnemyDisplayText.text += $"Emissivity: {RandomParam(CurrentEnemy.Emissivity, CurrentEnemy.EmissivitySTD):F3}";
    }






    private float R(float center)
    {
        return Random.Range(center * 0.75f, center * 1.25f);
    }

    private int IntRange(int min, int max)
    {
        return Random.Range(min, max + 1);
    }

    private IEnumerator ChallengeSpawning()
    {
        UpdateEnemyDisplayedText();
        yield return new WaitForSeconds(R(5f));
        Shuffle(waveMissileLevels);

        for (int i = 0; i < waveMissileLevels.Length; i++)
        {
            WaveDisplay.text = $"Wave {i+1}/{waveMissileLevels.Length}";

            SpawnEnemy(waveMissileLevels[i]);

            // Wait until player has shot down the missile
            while (waitingForPlayerToShootMissile)
            {
                yield return null;
            }

            // TODO: Check if player was correct or not


            CurrentEnemy = null;
            UpdateEnemyDisplayedText();

            yield return new WaitForSeconds(R(3f));
        }


        // Player has won!
        Debug.Log("Player wins!");
    }

    public void MissileReachedTheCenter()
    {
        StopAllCoroutines();
        // Show the game over state
        Debug.Log("Player loses!");
    }

    public void SpawnEnemy(int enemyLevel)
    {
        waitingForPlayerToShootMissile = true;


        if (enemyLevel == 1)
        {
            Shuffle(Enemies_LevelOne);
            CurrentEnemy = Enemies_LevelOne[0];
        }
        else if (enemyLevel == 2)
        {
            Shuffle(Enemies_LevelTwo);
            CurrentEnemy = Enemies_LevelTwo[0];
        } 
        else
        {
            Shuffle(Enemies_LevelThree);
            CurrentEnemy = Enemies_LevelThree[0];
        }

        UpdateEnemyDisplayedText();

        float spawnRadius = 49f;
        // Choose spawn point
        Vector3 spawnPosition = Random.insideUnitCircle.normalized * spawnRadius;

        // Make sure spawn point matches manufacturer
        float threshold = 20f;
        if (CurrentEnemy.isAmerican)
        {
            while (spawnPosition.x > -threshold)
            {
                spawnPosition = Random.insideUnitCircle.normalized * spawnRadius;
            }
        } 
        else
        {
            while (spawnPosition.x < threshold)
            {
                spawnPosition = Random.insideUnitCircle.normalized * spawnRadius;
            }
        }

        Quaternion spawnRotation = Quaternion.LookRotation(Vector3.forward, -spawnPosition);
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);
        newEnemy.GetComponent<Enemy>().speed = 0.05f;
        newEnemy.GetComponent<Enemy>().amArcadeMissile = false;
    }

    public static void Shuffle(EnemyData[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            EnemyData temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    public static void Shuffle(ushort[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            ushort temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}
