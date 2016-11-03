using UnityEngine;
using HoloToolkit.Unity;
using System.Collections.Generic;

public class FoliageSpawnManager : Singleton<FoliageSpawnManager> {
    [System.Serializable]
    public class FoliageSpawnSettings
    {
        [Tooltip("The maximum amount of plants to spawn")]
        public int maximumPlants = 40;

        [Tooltip("The delay in seconds in between each spawn of new foliage")]
        public float spawnDelay = 1f;

        [Tooltip("The spread in degrees for the randomly generated rays")]
        public float randomRaySpread = 20.0f;

        [Tooltip("The layer mask to use to use for ray-to-scene collisions")]
        public int layerMask = 31;
    }

    [Tooltip("Tweakable parameters for the frequency and behavior of spawning foliage")]
    public FoliageSpawnSettings foliageSpawnSettings;

    [Tooltip("The list of foliage available to spawn from. Includes prefabs and preferred orientation")]
    public Foliage[] spawnableFoliage;

    private float timecount = 0f;
    private Transform cachedTransform;

    //Prefilter the foliage by type into lists. The reason for this
    //Is to make it faster to extract the correct piece of foliage during run time.
    //Since there are really only two types, I just use two arrays. If there were many more,
    //the structure would be different to take advantage of FoliageType being an enum
    private List<GameObject> groundFoliageList;
    private List<GameObject> wallFoliageList;

    private int nPlants = 0;

	void Start () {
        ExtractFoliage();
        cachedTransform = transform;
	}
	
	void LateUpdate () {
        if (nPlants >= foliageSpawnSettings.maximumPlants)
            return;

        timecount += Time.deltaTime;

        if (timecount < foliageSpawnSettings.spawnDelay)
            return;

        CastFoliageRay();

        timecount = 0f;
	}

    public void Clear()
    {
        var children = new List<GameObject>(nPlants);

        foreach (Transform child in cachedTransform)
            children.Add(child.gameObject);

        foreach (GameObject child in children)
            Destroy(child);

        nPlants = 0;
    }

    /// <summary>
    /// Rearranges the designer-defined foliage prefabs by surface type.
    /// This simple logic is sufficient since there will only be two surface types,
    /// but an array or map of lists could be used if many more will be used
    /// </summary>
    private void ExtractFoliage()
    {
        int n = spawnableFoliage.Length;

        groundFoliageList = new List<GameObject>(n);
        wallFoliageList = new List<GameObject>(n);

        foreach(var foliage in spawnableFoliage)
        {
            if (foliage.type == Foliage.FoliageType.Ground)
                groundFoliageList.Add(foliage.template);
            else
                wallFoliageList.Add(foliage.template);
        }
    }

    /// <summary>
    /// Generates a random ray relatively within the player's
    /// FOV and casts it into the scene to create a new piece of foliage
    /// </summary>
    /// <param name="hitInfo"></param>
    /// <returns></returns>
    private void CastFoliageRay()
    {
        var playerTransform = Camera.main.transform;
        var origin = playerTransform.position;
        var dir = playerTransform.forward;

        //Rotate the direction to give some randomness to the spread
        float halfRotation = foliageSpawnSettings.randomRaySpread * 0.5f;
        float pitch = Random.Range(-halfRotation, halfRotation);
        float yaw = Random.Range(-halfRotation, halfRotation);
        Matrix4x4 spreadMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(new Vector3(pitch, yaw, 0f)), Vector3.one);
        
        dir = spreadMatrix.MultiplyPoint(dir).normalized;

        //Cast a ray into the "DEFAULT" layer. This avoids foliage and other elements in the scene
        RaycastHit hitInfo;
        if (Physics.Raycast(origin, dir, out hitInfo, 10f, ~foliageSpawnSettings.layerMask))
        {
            SpawnNewFoliage(hitInfo);
        }
    }
    private void SpawnNewFoliage(RaycastHit hitInfo)
    {
        var type = (hitInfo.normal.y > 0.7f) ? Foliage.FoliageType.Ground : Foliage.FoliageType.Wall;
        var foliage = CreateFoliage(type);
        var foliageTransform = foliage.transform;

        foliageTransform.SetParent(cachedTransform);
        foliageTransform.position = hitInfo.point;
        foliageTransform.up = hitInfo.normal;

        //Make ground foliage face the player
        if(type == Foliage.FoliageType.Ground)
        {
            var playerPos = Camera.main.transform.position;
            foliage.transform.LookAt(new Vector3(playerPos.x,
                                                 foliageTransform.position.y,
                                                 playerPos.z));
        }

        nPlants++;
    }
    private GameObject GetRandomPrefab(List<GameObject> list)
    {
        if (list == null)
            return null;

        int n = list.Count;

        if (n == 0)
            return null;

        int i = Random.Range(0, n);
        return list[i];
    }
    private GameObject CreateFoliage(Foliage.FoliageType type)
    {
        var prefab = (type == Foliage.FoliageType.Ground)
            ? GetRandomPrefab(groundFoliageList) : GetRandomPrefab(wallFoliageList);

        return Instantiate(prefab);
    }
}
