using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyDescription : MonoBehaviour
{
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Distance;
    public TextMeshProUGUI Speed;
    public TextMeshProUGUI CrossSection;
    public TextMeshProUGUI Emissivity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDescription(int ID, int Dist, int Vel, float Cross, float Emiss)
    {
        Title.text = $"Missile #{ID}";
        Distance.text = $"Distance: {Dist} km";
        Speed.text = $"Speed: {Vel} km/s";
        CrossSection.text = $"Radar Cross Section: {Cross:F2} m^2";
        Emissivity.text = $"Emissivity: {Emiss:F2}";
    }
}
