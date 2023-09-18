using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmpiresCore;
using EmpiresCore.WorldGeneration;
using System.Linq;

[RequireComponent(typeof(LiteLineRenderer))]
public class BorderRenderer : MonoBehaviour
{
    public int zOffset = 1;
    public bool drawCoast = true;
    public bool drawLandBorders = false;
    public float leftWidth = 1;
    public float rightWidth = 1;
    public float rightZOffset = 0;
    public bool widthShadowEffect = false;
    public int levelOfDetail = 4;
    public Color32 color = Color.black;
    public bool dash = false;
    public bool addCaps = true;

    private LiteLineRenderer lineRenderer;


    private void Awake ()
    {
        lineRenderer = GetComponent<LiteLineRenderer>();   
    }

    public void UpdateBorders (IEnumerable<Province> provinces)
    {
        foreach(Province province in provinces)
        {
            AddProvinceBorders(province);   
        }


        lineRenderer.Apply();
    }

    private void AddProvinceBorders (Province province)
    {
        for(int i = 0; i < province.Borders.Length; i++)
        {
            Border border = province.Borders[i];
            bool flip = province.BorderFlips[i];
            bool offset = province.Offsets[i];

            bool borderOwned = border.GetNeighbour(province) == null 
                || province.MeanPos.X > border.GetNeighbour(province).MeanPos.X;

            bool borderDrawn = (drawCoast && border.IsCoast) || (drawLandBorders && border.IsLandBorder);

            if(borderOwned && borderDrawn)
            {
                IEnumerable<Vec2> vertices = border.GetVertices(flip, offset, levelOfDetail);
                AddVertices(vertices, Vector3.zero);
                //AddVertices(vertices, Vector3.right * 256);
            }
        }
    }

    private void AddVertices (IEnumerable<Vec2> vertices, Vector3 offset)
    {
        List<Vec2> verts = vertices.ToList();
        Vector3 lastVert = Vector3.zero;
        bool isFirst = true;
        bool join = false;
        bool skip = dash;

        for(int i = 0; i < verts.Count; i++)
        {
            Vector3 vert = new Vector3(verts[i].X, verts[i].Y, -zOffset / 1000f);

            if(!isFirst && !skip)
            {
                float lineLength = Vector3.Distance(vert, lastVert);

                float left = leftWidth;
                float right = rightWidth;

                float scale = ProjectionUtilities.CalculateMercatorScaleFactor(vert.y / 256);

                if(widthShadowEffect)
                {
                    scale *= Mathf.Clamp01((vert.x - lastVert.x) / lineLength) * 0.5f + 0.5f;
                }

                bool startCap = addCaps && (i == 1 || dash);
                bool endCap = addCaps && (i == verts.Count - 1 || dash);

                left *= scale;
                right *= scale;
                lineRenderer.AddLine(lastVert + offset, vert + offset, left, right, 
                    rightZOffset, join, startCap, endCap);
                join = !dash;
                skip = dash;
            }
            else if(skip)
            {
                skip = false;
            }
            lastVert = vert;
            isFirst = false;
        }
    }
}
