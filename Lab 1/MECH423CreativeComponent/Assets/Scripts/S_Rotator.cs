using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_Rotator : MonoBehaviour
{

    [SerializeField]
    public Vector3 RestingRotation = Vector3.zero;
    public int averagingSize = 100;
    public float maxDeflectionDegree = 90f;

    private SerialScanner scanner;
    private Transform ControlledTransform;

    private List<Vector3> PreviousValues;

    // Start is called before the first frame update
    void Start()
    {
        // Search for a scanner
        scanner = FindAnyObjectByType<SerialScanner>();

        ControlledTransform = transform;
        ControlledTransform.rotation = Quaternion.Euler(RestingRotation);

        PreviousValues = new List<Vector3>();
    }

    // FixedUpdate is called 50 times per second
    void FixedUpdate()
    {
        if (scanner != null && scanner.HasAccelData())
        {
            PreviousValues.Add(scanner.GetUpVectorUnity().normalized);

            if (PreviousValues.Count > averagingSize)
            {
                // Remove oldest value
                PreviousValues.RemoveAt(0);
            }

            Vector3 averagedUpVector = Vector3.zero;
            for (int i = 0; i < PreviousValues.Count; i++)
            {
                averagedUpVector += PreviousValues[i];
            }

            averagedUpVector /= PreviousValues.Count;

            ControlledTransform.rotation = RotateFromBoardUp(averagedUpVector);
        }
    }

    private Quaternion RotateFromBoardUp(Vector3 boardUp)
    {
        boardUp.Normalize();

        // Calculate the angle between the up axis (Vector3.up) and boardUp
        float angleBetween = Vector3.Angle(Vector3.up, boardUp);

        // If the angle exceeds the max deflection degree, clamp it
        if (angleBetween > maxDeflectionDegree)
        {
            // Calculate the axis of rotation
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, boardUp).normalized;

            // Create a clamped target rotation from the up axis, using the max deflection degree
            Quaternion clampedRotation = Quaternion.AngleAxis(maxDeflectionDegree, rotationAxis);

            // Apply the clamped rotation to the up axis (Vector3.up)
            boardUp = clampedRotation * Vector3.up;
        }

        // Calculate the rotation needed to align the object's up vector with the clamped boardUp vector
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, boardUp);

        // Combine the target rotation with the resting rotation to preserve base orientation
        Quaternion finalRotation = targetRotation * Quaternion.Euler(RestingRotation);

        return finalRotation;
    }
}
