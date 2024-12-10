using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    
    public GameObject enemyPrefab;

    [Header("Game Progression")]
    public float enemyStartingSpeed = 0.4f;
    public EnemyData[] Enemies_LevelOne;
    public EnemyData[] Enemies_LevelTwo;
    public EnemyData[] Enemies_LevelThree;
    public ushort[] waveMissileLevels;

    [Header("Game UI")]
    public TextMeshProUGUI EnemyDisplayText;
    public TextMeshProUGUI WaveDisplay;
    public TextMeshProUGUI TimerDisplay;
    public TextMeshProUGUI ScoreDisplay;

    [Header("End of Round")]
    public GameObject EndOfRound_VisualCollection;
    public TextMeshProUGUI EndOfRound_ScoreDisplay;
    public TextMeshProUGUI EndOfRound_Instructions;


    [Header("Hands Off")]
    public bool waitingForPlayerToShootMissile;
    public EnemyData CurrentEnemy = null;

    private MissileLauncher MissileLauncher;
    private SerialScanner SerialScanner;
    
    private float startTime;
    private uint totalScore = 0;
    // Start is called before the first frame update
    void Start()
    {
        MissileLauncher = FindObjectOfType<MissileLauncher>();
        SerialScanner = FindObjectOfType<SerialScanner>();

        EndOfRound_VisualCollection.gameObject.SetActive(false);

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


    private void EndOfRound()
    {
        uint finalScore = totalScore;
        float totalTime = Time.time - startTime;

        int minutes = Mathf.FloorToInt(totalTime / 60);
        int seconds = Mathf.FloorToInt(totalTime % 60);
        TimerDisplay.text = $"{minutes:00}:{seconds:00}";

        EndOfRound_ScoreDisplay.text = $"Final score: {finalScore}\nTime: {minutes:00}:{seconds:00}";

        SerialScanner.HorizontalLine();
        SerialScanner.ThermalPrinter_WriteLine("Challenge Complete");
        SerialScanner.ThermalPrinter_WriteLine($"Final score: {finalScore}");
        SerialScanner.ThermalPrinter_WriteLine($"Time: {minutes:00}:{seconds:00}:");

        if (finalScore >= SerialScanner.HighScore_Challenge)
        {
            EndOfRound_ScoreDisplay.text += $"\nNew high score!";
            SerialScanner.ThermalPrinter_WriteLine("New high score!");
            SerialScanner.HighScore_Challenge = finalScore;
        }

        if (SerialScanner.BestTime_Challenge >= totalTime && finalScore >= 400)
        {
            EndOfRound_ScoreDisplay.text += $"\nNew best time!";
            SerialScanner.ThermalPrinter_WriteLine("New best time!");
            SerialScanner.BestTime_Challenge = totalTime;
        }

        SerialScanner.FinishChallengeReceipt();
        EndOfRound_Instructions.text = "";

        EndOfRound_VisualCollection.gameObject.SetActive(true);
        StartCoroutine(HandleGameOverFlow_Coroutine());
    }

    private IEnumerator HandleGameOverFlow_Coroutine()
    {
        yield return new WaitForSeconds(0.5f);
        uint baseFireCommands = SerialScanner.RxdLaunchCommands;
        long baseEncoder = SerialScanner.CummulativeEncoderCounts;
        bool mainMenu = true;
        while (true)
        {
            // If dial rotates, change the selection
            if (Mathf.Abs(SerialScanner.CummulativeEncoderCounts - baseEncoder) > 2 || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                mainMenu = !mainMenu;
                baseEncoder = SerialScanner.CummulativeEncoderCounts;
            }

            // Display selection
            if (mainMenu)
            {
                EndOfRound_Instructions.text = "-> Main Menu <-\n-- Play Again --";
            }
            else
            {
                EndOfRound_Instructions.text = "-- Main Menu --\n-> Play Again <-";
            }

            // If fire button pressed, load selection
            if (SerialScanner.RxdLaunchCommands > baseFireCommands || Input.GetKeyDown(KeyCode.Space))
            {
                if (mainMenu)
                {
                    SceneManager.LoadScene("MainMenu");
                }
                else
                {
                    SceneManager.LoadScene("Game");
                }
            }
            yield return null;
        }
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
        yield return new WaitForSeconds(R(1f));
        //Shuffle(waveMissileLevels);

        for (int i = 0; i < waveMissileLevels.Length; i++)
        {
            WaveDisplay.text = $"Wave {i+1}/{waveMissileLevels.Length}";

            yield return new WaitForSeconds(R(3f));

            SpawnEnemy(waveMissileLevels[i]);

            // Wait until player has shot down the missile
            while (waitingForPlayerToShootMissile)
            {
                yield return null;
            }

            CurrentEnemy = null;
            UpdateEnemyDisplayedText();
        }


        // Player has won!
        Debug.Log("Player wins!");
        EndOfRound();
    }

    public void PlayerShotMissile(Missile missile)
    {
        uint score = 0;
        uint addedPoints = 0;
        // TODO: Check if player was correct or not
        SerialScanner.ThermalPrinter_WriteLine("Missile shot down");
        SerialScanner.ThermalPrinter_WriteLine($"{CurrentEnemy.name}");

        SerialScanner.ThermalPrinter_WriteLine($"   Trajectory Module:");
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
            SerialScanner.ThermalPrinter_WriteLine($"- Iron rich: +{addedPoints}");
            score += addedPoints;
        }
        else
        {
            SerialScanner.ThermalPrinter_WriteLine($"- Iron rich set wrong: +0");
        }
        

        if (missile.BS3_HeavyMass == CurrentEnemy.BS3_HeavyMass)
        {
            addedPoints = 5;
            SerialScanner.ThermalPrinter_WriteLine($"- Heavy mass: +{addedPoints}");
            score += addedPoints;
        }
        else
        {
            SerialScanner.ThermalPrinter_WriteLine($"- Heavy mass set wrong: +0");
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
        

        SerialScanner.ThermalPrinter_WriteLine($"   Guidance Module:");
        if (CurrentEnemy.usesModule2)
        {
            if (missile.usesModule2)
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
                SerialScanner.ThermalPrinter_WriteLine($"- Guidance Module should not be disabled: +0");
            }

        }
        else
        {
            if (missile.usesModule2)
            {
                SerialScanner.ThermalPrinter_WriteLine($"- Guidance Module should not have been enabled: +0");
            }
            else
            {
                addedPoints = 40;
                SerialScanner.ThermalPrinter_WriteLine($"- Guidance Module correctly disabled: +{addedPoints}");
                score += addedPoints;
            }
        }

        SerialScanner.ThermalPrinter_WriteLine($"   Warhead Module:");
        if (CurrentEnemy.usesModule3)
        {
            if(missile.usesModule3)
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
                SerialScanner.ThermalPrinter_WriteLine($"- Warhead Module should not be disabled: +0");
            }

    }
        else
        {
            if (missile.usesModule3)
            {
                SerialScanner.ThermalPrinter_WriteLine($"- Warhead Module should not have been enabled: +0");
            }
            else
            {
                addedPoints = 40;
                SerialScanner.ThermalPrinter_WriteLine($"- Warhead Module correctly disabled: +{addedPoints}");
                score += addedPoints;
            }
        }

        SerialScanner.ThermalPrinter_WriteLine("");
        SerialScanner.ThermalPrinter_WriteLine($"Total missile score: {score}/100");
        SerialScanner.ThermalPrinter_FinishParagraph();


        totalScore += score;
        waitingForPlayerToShootMissile = false;
    }

    public void MissileReachedTheCenter()
    {
        StopAllCoroutines();
        // Show the game over state
        Debug.Log("Player loses!");
        EndOfRound();
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
        newEnemy.GetComponent<Enemy>().speed = enemyStartingSpeed / ((float)enemyLevel);
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
