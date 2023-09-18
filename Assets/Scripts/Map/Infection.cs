using EmpiresCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Infection : MonoBehaviour
{
    public static Infection Instance { get; private set; }

    private HashSet<Province> infected = new HashSet<Province>();
    private List<Province> atRisk = new List<Province>();

    private void Awake ()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start ()
    {
        DayCounter.Instance.DayIncremented += OnDayIncremented;
    }

    private void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            var province = WorldBuilder.Instance.GetProvinceAtPoint(MapCameraController.Instance.GetMousePosition());
            if(province != null)
            {
                Spawn(province);
            }
        }
    }

    private void OnDayIncremented (object sender, System.EventArgs e)
    {
        Spread();
    }

    public void Spawn (Province province)
    {
        Infect(province);
    }

    private void Infect (Province province)
    {
        if(infected.Contains(province))
        {
            return;
        }
        infected.Add(province);
        WorldBuilder.Instance.GetRegion(province).SetUVs(province, Region.ProvinceUV.One);
        AddNeighboursToRisk(province);
    }

    private void Spread ()
    {
        var oldRisk = atRisk;
        atRisk = new List<Province>();
        foreach(Province province in oldRisk)
        {
            Infect(province);
        }
    }

    private void AddNeighboursToRisk (Province p)
    {
        foreach(Province neighbour in p.Borders.Select(b => b.GetNeighbour(p)).Where(n => n != null))
        {
            if(!infected.Contains(neighbour))
            {
                atRisk.Add(neighbour);
                WorldBuilder.Instance.GetRegion(neighbour).SetUVs(neighbour, Region.ProvinceUV.WorldSpace);
            }
        }
    }


    public bool IsProvinceInfected (Province province)
    {
        return infected.Contains(province);
    }
    

}
