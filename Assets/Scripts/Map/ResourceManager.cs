using EmpiresCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    public enum Resource
    {
        None = 0,
        Town = 1,
        Loot = 2,
        Fort = 3,
        Food = 4,
        Other = 5,
    }

    [SerializeField]
    private Texture townTexture;
    [SerializeField]
    private Texture lootTexture;
    [SerializeField]
    private Texture foodTexture;
    [SerializeField]
    private Texture questTexture;
    [SerializeField]
    private Texture fortTexture;
    [SerializeField]
    private Texture otherTexture;

    private Dictionary<Province, Resource> resources = new Dictionary<Province, Resource>();
    
    [SerializeField]
    private Transform iconPrefab;

    private List<Transform> icons = new List<Transform>();
    private Dictionary<Province, Transform> provinceIcons = new Dictionary<Province, Transform>();

    [SerializeField]
    private float iconSize = 0.06f;
    [SerializeField]
    private float maxIconSize = 2.5f;

    private HashSet<Province> expended = new HashSet<Province>();

    private Dictionary<Province, int> safeRests = new Dictionary<Province, int>();


    private void Awake ()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private Texture GetTexture (Resource resource)
    {
        switch(resource)
        {
            case Resource.None:
                return null;
            case Resource.Town:
                return townTexture;
            case Resource.Loot:
                return lootTexture;
            case Resource.Fort:
                return fortTexture;
            case Resource.Food:
                return foodTexture;
            case Resource.Other:
                return otherTexture;
            default:
                return null;
        }
    }    

    private Resource GetResource ()
    {
        float value = Random.value;

        return value < 0.6f ? Resource.None
            : value < 0.78f ? Resource.Town
            : value < 0.86f ? Resource.Fort
            : value < 0.93f ? Resource.Loot
            : Resource.Other;
    }

    private void Reveal (Province province)
    {
        var resource = GetResource();
        resources.Add(province, resource);

        if(resource == Resource.None)
        {
            return;
        }
        var pos = CaravanController.GetProvincePos(province);

        Transform icon = Instantiate(iconPrefab, 
            new Vector3(pos.x, pos.y, transform.position.z), 
            Quaternion.identity, 
            transform);

        icon.GetComponent<MeshRenderer>().material.mainTexture = GetTexture(resource);

        provinceIcons.Add(province, icon);
        icons.Add(icon);
    }

    private void LateUpdate ()
    {
        foreach(Transform icon in icons)
        {
            if(MapCameraController.Instance != null)
            {
                icon.localPosition = MapCameraController.Instance.GetWrappedPosition(icon.localPosition);

                float size = Mathf.Min(MapCameraController.Instance.Camera.orthographicSize * iconSize, maxIconSize);

                icon.localScale = Vector2.one * size;
                icon.gameObject.SetActive(true);
            }
            else
            {
                icon.gameObject.SetActive(false);
            }

            
        }
    }

    public void RemoveResource(Province p)
    {
        resources.Remove(p);
        resources.Add(p, Resource.None);

        if(provinceIcons.TryGetValue(p, out Transform icon))
        {
            icons.Remove(icon);
            Destroy(icon.gameObject);
            provinceIcons.Remove(p);
        }
    }

    public void ExpendResource (Province province)
    {
        expended.Add(province);
    }

    public bool IsExpended (Province province)
    {
        return expended.Contains(province);
    }

    public void AddSafeRests (Province province, int count)
    {
        if(safeRests.TryGetValue(province, out int old))
        {
            safeRests.Remove(province);
            count += old;
        }

        safeRests.Add(province, count);
    }

    public bool IsRestSafe (Province province, bool expendUse)
    {
        if(safeRests.TryGetValue(province, out int old))
        {
            if(old > 0)
            {
                if(expendUse)
                {
                    safeRests.Remove(province);
                    safeRests.Add(province, old - 1);
                }
                return true;
            }            
        }
        return false;
    }

    public void Explore(Province province, int range)
    {
        int i = 0;
        List<Province> toExplore = new List<Province>() { province };

        if(!resources.ContainsKey(province))
        {
            resources.Add(province, Resource.None);
        }

        while(toExplore.Count > 0 && i < range)
        {
            i++;
            List<Province> newExplore = new List<Province>();

            foreach(Province p in toExplore)
            {
                foreach(Province neighbour in p.Borders
                    .Select(b => b.GetNeighbour(p))
                    .Where(n => n != null))
                {
                    if(!resources.ContainsKey(neighbour))
                    {
                        Reveal(neighbour);
                    }
                    newExplore.Add(neighbour);
                }
            }

            toExplore = newExplore;
        }
        
    }

    public Resource GetProvinceResource (Province province)
    {
        if(resources.TryGetValue(province, out Resource value))
        {
            return value;
        }
        return Resource.None;
    }

    public static string GetDescription (Resource resource)
    {
        switch(resource)
        {
            case Resource.None:
                return "Nothing";
            case Resource.Town:
                return "A small town";
            case Resource.Loot:
                return "Possible loot";
            case Resource.Fort:
                return "A fortress";
            case Resource.Food:
                return "A source of food";
            default:
                return "Unknown";
        }
    }
}
