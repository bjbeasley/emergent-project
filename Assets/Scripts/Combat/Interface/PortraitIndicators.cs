using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PortraitIndicators : MonoBehaviour
{
    [SerializeField]
    private Transform heartPrefab;
    [SerializeField]
    private Transform shieldPrefab;
    [SerializeField]
    private float zOffset = -2;

    private List<HeartIcon> hearts;
    private List<ShieldIcon> shields;

    private Character character;

    [SerializeField]
    private float evenOffset = 0.5f;

    public void Initialise (Character character)
    {
        this.character = character;
        hearts = new List<HeartIcon>();
        shields = new List<ShieldIcon>();
    }



    // Update is called once per frame
    void LateUpdate()
    {
        if(character == null)
        {
            return;
        }
        if(character.MaxHP != hearts.Count)
        {
            UpdateHeartCount();
            UpdateHeartPositions();           
        }
        if(character.ShieldPoints + character.TempShieldPoints != shields.Count)
        {
            UpdateShieldCount();
            UpdateShieldPositions();
        }
        UpdateHeartFill();
        UpdateShieldColor();


    }

    private void UpdateHeartCount ()
    {
        while(hearts.Count > character.MaxHP)
        {
            Destroy(hearts[hearts.Count - 1].gameObject);
            hearts.RemoveAt(hearts.Count - 1);
        }
        while(hearts.Count < character.MaxHP)
        {
            HeartIcon newHeart = Instantiate(heartPrefab, transform).GetComponent<HeartIcon>();

            hearts.Add(newHeart);
        }
    }

    private void UpdateShieldCount ()
    {
        int count = character.ShieldPoints + character.TempShieldPoints;
        while(shields.Count > count)
        {
            Destroy(shields[shields.Count - 1].gameObject);
            shields.RemoveAt(shields.Count - 1);
        }
        while(shields.Count < count)
        {
            var newShield = Instantiate(shieldPrefab).GetComponent<ShieldIcon>();

            shields.Add(newShield);
            newShield.transform.parent = transform;
        }
    }

    private void UpdateHeartPositions ()
    {
        int index = 0;

        foreach(var pos in GetPositions(hearts.Count, true))
        {
            hearts[index].transform.localPosition = pos;
            index++;
        }

        hearts = hearts.OrderBy(h => h.transform.position.z).ToList();
    }

    private void UpdateShieldPositions ()
    {
        int index = 0;

        foreach(var pos in GetPositions(shields.Count, false))
        {
            shields[index].transform.localPosition = pos;
            index++;
        }

        shields = shields.OrderBy(h => h.transform.position.z).ToList();
    }

    private IEnumerable<Vector3> GetPositions (float count, bool bottom)
    {
        bool even = count % 2 == 0;
        float adjustedCount = count + (even ? evenOffset : 0);

        float angleDelta = Mathf.Min(Mathf.PI / 12, Mathf.PI * 0.75f / adjustedCount);
        float start = Mathf.PI * 1.5f - angleDelta * (adjustedCount / 2f);
        float r = 0.677f;

        for(int i = 0; i < count; i++)
        {
            float indexPos = i;
            if(even && i >= count / 2)
            {
                indexPos = i + evenOffset;
            }

            float angle = start + angleDelta * (indexPos + 0.5f);
            Vector3 pos = new Vector3(
                Mathf.Cos(angle) * r,
                Mathf.Sin(angle) * r * (bottom ? 1 : -1),
                zOffset + Mathf.Abs(adjustedCount / 2f - i - 0.5f) / (adjustedCount) + (even && i >= count / 2 ? evenOffset / 2 : 0)) ;

            yield return pos;
        }
    }

    private void UpdateHeartFill ()
    {
        for(int i = 0; i < hearts.Count; i++)
        {
            hearts[i].SetFilled(i < character.HP);
        }
    }

    private void UpdateShieldColor ()
    {
        
        for(int i = 0; i < shields.Count; i++)
        {
            shields[i].SetTemp(i < character.TempShieldPoints);
        }
    }
}
