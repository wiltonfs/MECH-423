using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowball : MonoBehaviour
{
    public float lifetime;
    public float killY = -3f;

    private float killTimer;
    // Start is called before the first frame update
    void Start()
    {
        killTimer = Time.time + lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > killTimer || transform.position.y < killY)
        {
            Destroy(this.gameObject);
        }
    }
}
