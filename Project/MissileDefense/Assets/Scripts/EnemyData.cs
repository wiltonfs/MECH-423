using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    [Header("General Info")]
    public string enemyName;
    public uint enemyID;
    public bool isAmerican;
    public uint RadarCrossSection;
    public uint RadarCrossSectionSTD;
    public uint Emissivity;
    public uint EmissivitySTD;

    [Header("Module 1")]
    public bool BS2_IronRich;
    public bool BS3_HeavyMass;
    public bool isCruiseMissile;

    [Header("Module 2")]
    public bool usesModule2;
    public ushort Slider1;
    public ushort Slider2;
    public bool FourState_1;
    public bool FourState_0;

    [Header("Module 3")]
    public bool usesModule3;
    public ushort StateMachine;
}
