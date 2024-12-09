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
    public TextMeshProUGUI ScoreDisplay;
    public ushort[] waveMissileLevels;

    public bool waitingForPlayerToShootMissile;


    private MissileLauncher MissileLauncher;
    private SerialScanner SerialScanner;
    public EnemyData CurrentEnemy = null;

    private float startTime;
    private uint totalScore = 0;
    // Start is called before the first frame update
    void Start()
    {
        MissileLauncher = FindObjectOfType<MissileLauncher>();
        SerialScanner = FindObjectOfType<SerialScanner>();

        startTime = Time.time;
        WaveDisplay.text = $"Wave 1/{waveMissileLevels.Length}";

        StartCoroutine(ChallengeSpawning());
        SerialScanner.StartChallengeReceipt();
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time - startTime;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        TimerDisplay.text = $"{minutes:00}:{seconds:00}";

        ScoreDisplay.text = $"Score: {totalScore}";
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

    public void PlayerShotMissile(Missile missile)
    {
        uint score = 0;
        uint addedPoints = 0;
        // TODO: Check if player was correct or not
        SerialScanner.ThermalPrinter_WriteLine("Missile shot down");
        SerialScanner.ThermalPrinter_WriteLine($"{CurrentEnemy.name}");

        SerialScanner.ThermalPrinter_WriteLine($"   Module 1:");
        if (missile.BS1_Coriolis == CurrentEnemy.isAmerican)
        {
            addedPoints = 5;
            SerialScanner.ThermalPrinter_WriteLine($"- Coriolis: +{addedPoints}");
            score += addedPoints;
        } else {
            SerialScanner.ThermalPrinter_WriteLine($"- Coriolis set wrong: +0");
        }
        

        if (missile.BS2_IronRich == CurrentEnemy.BS2_IronRich)
        {
            addedPoints = 5;
            SerialScanner.ThermalPrinter_WriteLine($"- Gyroscope: +{addedPoints}");
            score += addedPoints;
        }
        else
        {
            SerialScanner.ThermalPrinter_WriteLine($"- Gyroscope set wrong: +0");
        }
        

        if (missile.BS3_HeavyMass == CurrentEnemy.BS3_HeavyMass)
        {
            addedPoints = 5;
            SerialScanner.ThermalPrinter_WriteLine($"- Boost: +{addedPoints}");
            score += addedPoints;
        }
        else
        {
            SerialScanner.ThermalPrinter_WriteLine($"- Boost set wrong: +0");
        }
        

        if (missile.isCruiseMissile == CurrentEnemy.isCruiseMissile)
        {
            addedPoints = 5;
            SerialScanner.ThermalPrinter_WriteLine($"- Cruise: +{addedPoints}");
            score += addedPoints;
        }
        else
        {
            SerialScanner.ThermalPrinter_WriteLine($"- Cruise set wrong: +0");
        }
        

        SerialScanner.ThermalPrinter_WriteLine($"   Module 2:");
        if (CurrentEnemy.usesModule2)
        {
            uint SLIDER_THRESHOLD = 50;
            if (Mathf.Abs(missile.Slider1 - CurrentEnemy.Slider1) <= SLIDER_THRESHOLD)
            {
                addedPoints = 15;
                SerialScanner.ThermalPrinter_WriteLine($"- QF: +{addedPoints}");
                score += addedPoints;
            }
            else
            {
                SerialScanner.ThermalPrinter_WriteLine($"- QF set wrong: +0");
            }

            if (Mathf.Abs(missile.Slider2 - CurrentEnemy.Slider2) <= SLIDER_THRESHOLD)
            {
                addedPoints = 15;
                SerialScanner.ThermalPrinter_WriteLine($"- AF: +{addedPoints}");
                score += addedPoints;
            }
            else
            {
                SerialScanner.ThermalPrinter_WriteLine($"- AF set wrong: +0");
            }

            if (missile.FourState_0 == CurrentEnemy.FourState_0 && missile.FourState_1 == CurrentEnemy.FourState_1)
            {
                addedPoints = 10;
                SerialScanner.ThermalPrinter_WriteLine($"- Thermal: +{addedPoints}");
                score += addedPoints;
            }
            else
            {
                SerialScanner.ThermalPrinter_WriteLine($"- Thermal set wrong: +0");
            }

        }
        else
        {
            if (missile.usesModule2)
            {
                SerialScanner.ThermalPrinter_WriteLine($"- Mod 2 should not have been enabled: +0");
            }
            else
            {
                addedPoints = 40;
                SerialScanner.ThermalPrinter_WriteLine($"- Mod 2 correctly disabled: +{addedPoints}");
                score += addedPoints;
            }
        }

        SerialScanner.ThermalPrinter_WriteLine($"   Module 3:");
        if (CurrentEnemy.usesModule3)
        {
            if (missile.StateMachine == CurrentEnemy.StateMachine)
            {
                addedPoints = 40;
                SerialScanner.ThermalPrinter_WriteLine($"- Warhead state machine: +{addedPoints}");
                score += addedPoints;
            }
            else
            {
                SerialScanner.ThermalPrinter_WriteLine($"- Warhead state machine set wrong: +0");
            }

        }
        else
        {
            if (missile.usesModule3)
            {
                SerialScanner.ThermalPrinter_WriteLine($"- Mod 3 should not have been enabled: +0");
            }
            else
            {
                addedPoints = 40;
                SerialScanner.ThermalPrinter_WriteLine($"- Mod 3 correctly disabled: +{addedPoints}");
                score += addedPoints;
            }
        }

        SerialScanner.ThermalPrinter_WriteLine("");
        SerialScanner.ThermalPrinter_WriteLine($"Total missile score: {score}/100");
        SerialScanner.ThermalPrinter_WriteLine("");


        totalScore += score;
        waitingForPlayerToShootMissile = false;
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
