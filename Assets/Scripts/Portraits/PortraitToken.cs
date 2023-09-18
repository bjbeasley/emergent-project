using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class PortraitToken : MonoBehaviour
{
    [SerializeField]
    private MeshFilter headMeshFilter;
    [SerializeField]
    private MeshFilter eyesMeshFilter;
    [SerializeField]
    private MeshFilter hairMeshFilter;

    [SerializeField]
    private MeshRenderer eyeMeshRenderer;

    [SerializeField]
    private SpriteRenderer ring;

    [SerializeField]
    private Color friendlyColor = Color.blue;
    [SerializeField]
    private Color enemyColor = Color.red;


    [SerializeField]
    private PortraitIndicators indicators;

    private int eyeTextureIndex;

    private Character character;
    private Color aliveColor;
    private Color deadColor;

    bool alive;

    private void Awake ()
    {
        eyeMeshRenderer.material = new Material(eyeMeshRenderer.material);
        eyeTextureIndex = eyeMeshRenderer.material.GetTexturePropertyNameIDs()[0];

    }

    public void AssignPortrait(Character character, bool friendly)
    {
        this.character = character;

        aliveColor = friendly ? friendlyColor : enemyColor;
        deadColor = Color.Lerp(aliveColor, Color.gray, 0.5f);
        alive = character.Awake;
        character.Portrait.EyesOpen = alive;
        character.Portrait.Generate();

        ring.color = (character.HP > 0 ? aliveColor : deadColor);

        headMeshFilter.mesh = character.Portrait.GetHeadMesh();
        eyesMeshFilter.mesh = character.Portrait.GetEyeMesh();
        hairMeshFilter.mesh = character.Portrait.GetHairMesh();


        if(indicators != null)
        {
            indicators.Initialise(character);
        }
        
    }

    private void LateUpdate ()
    {
        Vector3 worldPos = transform.position;

        var cam = CombatCamera.Instance != null ? CombatCamera.Instance : (CameraController)MapCameraController.Instance;
        Vector3 cameraPos = cam.GetMousePosition();

        Vector2 delta = cameraPos - worldPos;
        Vector2 offset = Vector2.ClampMagnitude(delta / 20, 0.1f);

        eyeMeshRenderer.material.SetTextureOffset(eyeTextureIndex, -offset);

        ring.color = (character.HP > 0 ? aliveColor : deadColor);

        transform.rotation = Quaternion.Euler(0, 0, (character.HP > 0 ? 0 : -30));

        if(alive != character.Awake)
        {
            alive = character.Awake;
            character.Portrait.EyesOpen = alive;
            character.Portrait.Generate();
        }
    }

    public void OnHover ()
    {
        string label = character.Name.First;

        if(character.Item != null)
        {
            label += "\nHolding: " + character.Item.Name + " (" + character.Item.ItemRarity + ")";
        }

        ActionTooltip.Instance.Show(label);
   
    }
}
