using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using EmpiresCore;
using EmpiresCore.WorldGeneration;
using EmpiresCore.WorldGeneration.Structures;
using System;

public class WorldBuilder : MonoBehaviour
{
    public static WorldBuilder Instance { get; private set; }
    
    public string voronoiFileName;
    public TileableVoronoiDiagram diagram;

    public Region regionPrefab;
    public int horizontalRegionCount = 8;
    public int verticalRegionCount = 4;

    private Region[,] regions;

    private Vector2Int mapSize;

    public bool Generated { get; private set; } = false;

    public event EventHandler<ProvinceClickedEventArgs> OnProvinceClicked;

    public float TimezoneOffset { get; private set; }

    public class ProvinceClickedEventArgs : EventArgs
    {
        public Province Province { get; }
        public ProvinceClickedEventArgs (Province province) { Province = province; }
    }


    // Start is called before the first frame update
    void Start()
    {

        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);

        UnitySystemConsoleRedirector.Redirect();
        Camera.main.backgroundColor = Color.red;
        Instance = this;
        StartCoroutine(GenerateWorldMap());
    }

    IEnumerator GenerateWorldMap ()
    {
        yield return StartCoroutine(LoadVoronoiDiagram());

        Camera.main.backgroundColor = Color.yellow;

        int seed = (int)(UnityEngine.Random.value * int.MaxValue);
        Debug.Log(seed);

        WorldGenerator worldGenerator = new WorldGenerator(seed);
        worldGenerator.GenerateWorldMapAsync(diagram);

        diagram = null;

        while(!worldGenerator.Done)
        {
            yield return null;
        }

        WorldMap worldMap = worldGenerator.Output;
        mapSize = new Vector2Int(worldMap.Width, worldMap.Height);

        regions = new Region[horizontalRegionCount, verticalRegionCount];

        for(int x = 0; x < horizontalRegionCount; x++)
        {
            for(int y = 0; y < verticalRegionCount; y++)
            {
                regions[x, y] = Instantiate(regionPrefab);
                regions[x, y].transform.parent = transform;
                regions[x, y].name = "region[" + x + ", " + y + "]";
            }
        }

        TimeProfiler.OutputCurrentTime("Regions instantiated");

        Task[] tasks = new Task[worldMap.Provinces.Length];
        int i = 0;
        foreach(Province province in worldMap.Provinces)
        {
            var indices = GetRegionIndices(province.MeanPos);

            tasks[i] = new Task(() => regions[indices.x, indices.y].AddProvince(province));
            tasks[i].Start();
            i++;
        }

        while(tasks.Any(task => !task.IsCompleted))
        {
            yield return null;
        }

        TimeProfiler.OutputCurrentTime("Regions triangulated");
        yield return null;

        foreach(Region region in regions)
        {
            region.UpdateMesh();
        }

        TimeProfiler.OutputCurrentTime("Meshes applied");

        foreach(Region region in regions)
        {
            region.UpdateBorders();
        }

        TimeProfiler.OutputCurrentTime("Lines applied");
        yield return null;

        Camera.main.backgroundColor = new Color(0.55f, 0.8f, 0.99f);
        TimeProfiler.OutputCurrentTime("Generation completed!");

        SpawnPointLocator spawnPointLocator = new SpawnPointLocator(worldMap.Provinces);

        spawnPointLocator.Start();

        while(!spawnPointLocator.Done)
        {
            yield return null;
        }

        TimeProfiler.OutputCurrentTime("Spawn points found");

        Province playerSpawn = spawnPointLocator.GetPlayerSpawn();
        TimezoneOffset = playerSpawn.MeanPos.X;
        CaravanController.Instance.SetProvince(playerSpawn);
        Infection.Instance.Spawn(spawnPointLocator.GetInfectionSpawn());

        Generated = true;
        MapCameraController.Instance.ZoomOnPlayer();
    }

    private Vector2Int GetRegionIndices (Vec2 pos)
    {
        int x = (int)(pos.X * 128 * horizontalRegionCount) / mapSize.x;
        int y = (int)(pos.Y * 128 * verticalRegionCount) / mapSize.y;

        x = x >= horizontalRegionCount ? horizontalRegionCount - 1 : x;

        return new Vector2Int(x, y);
    }

    public Region GetRegion (Province p)
    {
        Vector2Int indices = GetRegionIndices(p.MeanPos);
        return regions[indices.x, indices.y];
    }

    IEnumerator LoadVoronoiDiagram ()
    {
        string path = GetStreamingAssetsPath() + voronoiFileName;

        if(path.Contains("://") || path.Contains (":///"))
        {
            TimeProfiler.OutputCurrentTime("Deserializing via WWW");
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(path);
            yield return www.SendWebRequest();

            byte[] bytes = www.downloadHandler.data;
            diagram = VoronoiSerializer.DeserializeBytes(bytes);
        }
        else
        {
            TimeProfiler.OutputCurrentTime("Deserializing via file stream");
            Stream stream = File.OpenRead(path);
            diagram = VoronoiSerializer.DeserializeStream(stream);
        }    
    }


#pragma warning disable CS0162 // Unreachable code detected
    public string GetStreamingAssetsPath ()
    {
#if UNITY_IOS
        return Application.dataPath + "/Raw/";
#endif
#if UNITY_WINDOWS || UNITY_EDITOR
        return Application.dataPath + "/StreamingAssets/";
#endif
#if UNITY_ANDROID
        return "jar:file://" + Application.dataPath + "!/assets/";
#endif
        return Application.dataPath + "/StreamingAssets/";

    }
#pragma warning restore CS0162 // Unreachable code detected

    public Province GetProvinceAtPoint (Vector2 point)
    {
        if(!Generated)
        {
            return null;
        }
        Vec2 p = new Vec2(point.x, point.y);

        GetRegionIndices(p);

        foreach(Region region in regions)
        {
            Province province = region.GetProvinceAtPosition(point);
            if(province != null)
            {
                return province;
            }
        }
        return null;
    }

    private void OnMouseDown ()
    {
        if(!Generated)
        {
            return;
        }

        var province = GetProvinceAtPoint(MapCameraController.Instance.GetMousePosition());

        if(province != null)
        {
            OnProvinceClicked?.Invoke(this, new ProvinceClickedEventArgs(province));
        }
    }

    private void OnMouseOver ()
    {
        var province = GetProvinceAtPoint(MapCameraController.Instance.GetMousePosition());

        if(province == null)
        {
            return;
        }

        string text = "";

        var player = CaravanController.Instance.Province;

        if(province.Borders.Any(b => b.GetNeighbour(province) == player))
        {
            text = "<b>Click to move to province.</b>";
        }

        var resource = ResourceManager.Instance.GetProvinceResource(province);

        if(resource != ResourceManager.Resource.None)
        {
            if(!string.IsNullOrEmpty(text))
            {
                text += "\n";
            }
            text += "<i>Contains: " + ResourceManager.GetDescription(resource) + "</i>";
        }

        if(province.Biome == Biome.Mountains)
        {
            if(!string.IsNullOrEmpty(text))
            {
                text += "\n";
            }
            text += "Mountainous! Takes 2 full days to travel across";
        }

        if(!string.IsNullOrEmpty(text))
        {
            ActionTooltip.Instance.Show(text);
        }


    }
}
