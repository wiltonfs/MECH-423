using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_LeanController : MonoBehaviour
{
    private Rigidbody2D Rigidbody;
    public float ForceScale = 0.1f;
    public float MaxVel = 0.5f;

    public SerialScanner SerialScanner;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SerialScanner != null && SerialScanner.HasAccelData())
        {
            Vector3 Accel = SerialScanner.ReadAccelBuffer();
            Vector2 Force = new Vector2(Accel.x, Accel.y);
            Rigidbody.AddForce(ForceScale * Force);

            // Clamp velocity
            Rigidbody.velocity = new Vector2(Mathf.Clamp(Rigidbody.velocity.x, -MaxVel, MaxVel), Mathf.Clamp(Rigidbody.velocity.y, -MaxVel, MaxVel));
        }
    }
}
