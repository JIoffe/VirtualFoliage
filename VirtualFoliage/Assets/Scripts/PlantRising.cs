using UnityEngine;
using System.Collections;

/// <summary>
/// Simple behavior of the plants rising out of the surfaces that they are created from
/// </summary>
public class PlantRising : MonoBehaviour {
    [Tooltip("The starting offset within the surface that the object will begin with. If it is a ground object, then this is the distance underground")]
    public float startingOffset = 1f;

    [Tooltip("The time in seconds until the plant has fully risen")]
    public float timeToRise = 1f;

    private bool hasRisen = false;
    private float runtime = 0f;

    private Vector3 targetPosition;
    private Vector3 startingPosition;

	void Start () {
        targetPosition = transform.position;
        startingPosition = transform.position - transform.up * startingOffset;
        transform.position = startingPosition;
	}
	
	void Update () {
        if (hasRisen)
            return;

        runtime += Time.deltaTime;

        if(runtime >= timeToRise)
        {
            transform.position = targetPosition;
            hasRisen = true;
            return;
        }

        float s = runtime / timeToRise;

        transform.position = Vector3.Lerp(startingPosition, targetPosition, s);
	}
}
