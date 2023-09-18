using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmpiresCore;
using System.Linq;
using System.Text;

public class CaravanController : MonoBehaviour
{
    private Province currentProvince;
    public Province Province { get { return currentProvince; } }

    public static CaravanController Instance { get; private set; }

    [SerializeField]
    private float scale = 0.1f;

    private bool moving = false;

    [SerializeField]
    private Transform pinTransform;

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

    void Start()
    {
        WorldBuilder.Instance.OnProvinceClicked += OnProvinceClicked;
    }

    public void SetProvince (Province province)
    {
        currentProvince = province;
        Vector2 pos = GetProvincePos(province);
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        ResourceManager.Instance.Explore(currentProvince, 2);
        moving = false;
    }

    private void OnProvinceClicked (object sender, WorldBuilder.ProvinceClickedEventArgs e)
    {
        Province province = e.Province;

        if(!moving && currentProvince == null || currentProvince.Borders.Any(b => b.GetNeighbour(currentProvince) == province))
        {
            TryMoveToProvince(province);
        }
    }

    void Update()
    {
        transform.position = MapCameraController.Instance.GetWrappedPosition(transform.position);
        transform.localScale = Vector2.one * MapCameraController.Instance.Camera.orthographicSize * scale;
    }

    private IEnumerator MoveCoroutine (Vector3 newPos, Province province)
    {
        moving = true;
        float pinY = pinTransform.localPosition.y;

        float t = 0;

        float duration = 0.3f;

        Vector3 startPos = transform.position;

        while(t <= duration && moving)
        {      
            yield return new WaitForEndOfFrame();
            float t2 = Mathf.Clamp01(Mathf.SmoothStep(0, 1, t / duration));

            Vector3 startPosWrapped = MapCameraController.Instance.GetWrappedPosition(startPos);
            Vector3 newPosWrapped = MapCameraController.Instance.GetWrappedPosition(newPos);

            Vector3 lerped = Vector3.Lerp(startPosWrapped, newPosWrapped, t2);
            transform.position = lerped;

            float yOffset = 1 - Mathf.Pow(2 * t2 - 1, 2);
            pinTransform.localPosition = new Vector3(pinTransform.localPosition.x, pinY + yOffset, pinTransform.localPosition.z);


            t += Time.unscaledDeltaTime;
        }
        if(moving)
        {
            transform.position = newPos;
            pinTransform.localPosition = new Vector3(pinTransform.localPosition.x, pinY, pinTransform.localPosition.z);
            moving = false;
        }
        Storyteller.Instance.GetProvinceEvent(province);

    }



    private void TryMoveToProvince (Province province, bool warnDowned = true)
    {
        if(moving)
        {
            return;
        }
        if(Storyteller.Instance.Party.Any(c => !c.Awake))
        {
            if(warnDowned)
            {
                WarnDowned(() => TryMoveToProvince(province, false));
                return;
            }
            else
            {
                Storyteller.Instance.RemoveDownedMembers();
            }
        }

        Vector2 pos = GetProvincePos(province);
        StartCoroutine(MoveCoroutine(new Vector3(pos.x, pos.y, transform.position.z), province));
        currentProvince = province;
        ResourceManager.Instance.Explore(currentProvince, 2);
        DayCounter.Instance.IncrementPhase();
        if(province.Biome == Biome.Mountains)
        {
            DayCounter.Instance.IncrementPhase();
            DayCounter.Instance.IncrementPhase();
            DayCounter.Instance.IncrementPhase();
        }

        

    }

    public static Vector2 GetProvincePos (Province province)
    {
        Vec2 pos = province.MeanPos;
        if(province.PointInsideProvince(pos))
        {
            return new Vector2(pos.X, pos.Y);
        }

        var verts = province.GetVertices(true, 2).ToList();

        for(int i = 0; i < verts.Count; i++)
        {
            Vec2 a = verts[i];
            Vec2 b = verts[(i + verts.Count / 2) % verts.Count];

            Vec2 mean = (a + b) / 2;

            if(province.PointInsideProvince(mean))
            {
                return new Vector2(mean.X, mean.Y);
            }
        }

        return new Vector2(pos.X, pos.Y);
    }

    private void WarnDowned (System.Action action)
    {
        Debug.Log("Warning player of party loss");
        IEnumerable<Character> downed = Storyteller.Instance.Party.Where(c => c.HP <= 0);

        StringBuilder sb = new StringBuilder();
        sb.Append("The following member(s) of your party are unconscious:");
        foreach(Character character in downed)
        {
            sb.Append("\n - ");
            sb.Append(character.Name);
        }
        sb.Append("\n\nIf you attempt to move now those characters will be left behind and lost to you.");
        sb.Append("\n\nConsider resting here to allow them to recover before you move.");

        DialogManager.Instance.CreateDialog(
            "Are You Sure?",
            sb.ToString(),
            new DialogOption("<b>Let me reconsider</b>"),
            new DialogOption("It must be done (they will leave your party <b>permanently</b>)", action));
    }
}
