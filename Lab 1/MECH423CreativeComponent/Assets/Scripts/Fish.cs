using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public int fishValue = 1;
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
        if (Time.time > killTimer || transform.position.z < killY)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Penguin penguin = other.gameObject.GetComponent<Penguin>();
        if (penguin != null)
        {
            penguin.CollectFish(fishValue);
            Destroy(this.gameObject);
        }
    }
}
