using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;



    [Header("Set At Spawn")]
    public bool BS1_Coriolis;
    public bool BS2_IronRich;
    public bool BS3_HeavyMass;
    public bool isCruiseMissile;

    public bool usesModule2;
    public ushort Slider1;
    public ushort Slider2;
    public bool FourState_1;
    public bool FourState_0;

    public bool usesModule3;
    public ushort StateMachine;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, Vector3.zero) > 50f)
        {
            Destroy(gameObject);
        }
    }
}
