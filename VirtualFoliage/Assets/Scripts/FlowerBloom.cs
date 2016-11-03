using UnityEngine;
using System.Collections;

/// <summary>
/// Assumes that that this mesh portion has a blend shape that corresponds to a
/// closed flower. This script will open the flower gradually over time.
/// </summary>
public class FlowerBloom : MonoBehaviour {
    [Tooltip("The time in seconds before the flower begins to bloom")]
    public float bloomDelay = 2f;

    [Tooltip("The time in seconds that this component will take to bloom")]
    public float bloomTime = 2f;

    private SkinnedMeshRenderer skinnedMeshRenderer;
    private bool hasBloomed = false;
    private float lifetime = 0f;
    private float targetTime;

	void Start () {
        targetTime = bloomDelay + bloomTime;
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
	}
	
	void Update () {
        if (hasBloomed)
            return;

        lifetime += Time.deltaTime;

        float bloomAmount = (lifetime - bloomDelay) / bloomTime;
        bloomAmount = 1f - Mathf.Clamp(bloomAmount, 0f, 1f);

        bloomAmount *= 100f;

        skinnedMeshRenderer.SetBlendShapeWeight(0, bloomAmount);

        hasBloomed = lifetime >= targetTime;
	}
}
