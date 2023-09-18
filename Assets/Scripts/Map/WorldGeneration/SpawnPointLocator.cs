using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmpiresCore;

public class SpawnPointLocator
{
    private HashSet<Province> explored = new HashSet<Province>();

    private List<Province> provinces;

    private int largestContinent = 0;

    private Province infectionSpawn;
    private Province playerSpawn;

    public bool Done { get; private set; } = false;

    public SpawnPointLocator(IEnumerable<Province> provinces)
    {
        this.provinces = provinces.ToList();
    }

    public void Start ()
    {
        foreach(Province province in provinces)
        {
            if(!explored.Contains(province))
            {
                ExploreContinent(province);
            }
        }

        Done = true;
    }

    private void ExploreContinent (Province start)
    {
        List<Province> exploring = new List<Province>() { start };
        int provinceCount = 0;
        Province lastExplored = null;

        while(exploring.Count > 0)
        {
            List<Province> nextToExplore = new List<Province>();

            foreach(Province province in exploring)
            {
                if(!explored.Contains(province))
                {
                    provinceCount++;
                    explored.Add(province);
                    lastExplored = province;

                    foreach(Province neighbour in province.Borders.Select(b => b.GetNeighbour(province)).Where(n => n != null))
                    {
                        if(!explored.Contains(neighbour))
                        {
                            nextToExplore.Add(neighbour);
                        }
                    }
                }
            }
            exploring = nextToExplore;
        }

        if(provinceCount > largestContinent)
        {
            largestContinent = provinceCount;
            infectionSpawn = lastExplored;
            playerSpawn = lastExplored.Borders
                .Select(b => b.GetNeighbour(lastExplored))
                .Where(n => n != null)
                .First();
        }
    }


    public Province GetInfectionSpawn ()
    {
        return infectionSpawn;
    }

    public Province GetPlayerSpawn ()
    {
        return playerSpawn;
    }

}

