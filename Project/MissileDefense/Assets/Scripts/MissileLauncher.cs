using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MissileLauncher : MonoBehaviour
{
    public Transform[] cities = new Transform[4];
    public float aimLineThickness = 2f;
    public float aimLineLength = 150f;
    public float aimSpeedCoarse = 10f;
    public float aimSpeedFine = 1f;

    private int activeCity = 0;
    private float aimSpeed = 0f;

    private LineRenderer aimLine;
    private float aimRotationDegrees = 0;
    // Start is called before the first frame update
    void Start()
    {
        // Configure aiming line
        aimLine = gameObject.GetComponent<LineRenderer>();
        aimLine.startColor = Color.red;
        aimLine.endColor = Color.red;
        aimLine.startWidth = aimLineThickness;
        aimLine.endWidth = aimLineThickness;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            aimSpeed = aimSpeedFine;
        } 
        else
        {
            aimSpeed = aimSpeedCoarse;
        }


        UpdateAimLine();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            activeCity = 0;
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            activeCity = 1;
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            activeCity = 2;
        } else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            activeCity = 3;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            aimRotationDegrees -= Time.deltaTime * aimSpeed;
        } else if (Input.GetKey(KeyCode.LeftArrow))
        {
            aimRotationDegrees += Time.deltaTime * aimSpeed;
        }
    }

    private void UpdateAimLine()
    {
        Vector3 direction = Quaternion.Euler(0, 0, aimRotationDegrees) * Vector3.right;
        Vector3 startPoint = cities[activeCity].position;
        Vector3 endPoint = startPoint + direction.normalized * aimLineLength;
        aimLine.positionCount = 2;
        aimLine.SetPosition(0, startPoint);
        aimLine.SetPosition(1, endPoint);
    }

}
