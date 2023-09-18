using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Mountain
{
    private Vector2[] points;

    private const float randomContourChance = 0.0f;

    private int peakIndex = 0;
    private int peakContourIndex = 0;

    private const float maxGradient = 1;

    private readonly float width;
    private readonly float baseHeight;
    private readonly int numVertices;
    private readonly float lineWidth;
    private readonly float yVariation;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> tris = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    private Vector3 position;

    private readonly Vector2 lightUV;
    private readonly Vector2 darkUV;
    private readonly Vector2 outlineUV;

    private float cos;
    private float sin;

    private const float contourLineZOffset = -0.0005f;
    private const float contourBaseZOffset = 0.0010f;
    private const float outlineZOffset = 0.0015f;

    private class Contour
    {
        public int Index { get; set; }
        public Vector2 EndPoint { get; set; }

        public Contour (int index, Vector2 endPoint)
        {
            Index = index;
            EndPoint = endPoint;
        }  
    }

    private List<Contour> contours = new List<Contour>();


    public Mountain (int seed, float width, float height, int numVertices,
        float lineWidth, float yVariation, Vector3 position,
        Vector2 lightUV, Vector2 darkUV, Vector2 outlineUV)
    {
        Random.InitState(seed);
        this.width = width;
        this.baseHeight = height;
        this.numVertices = numVertices;
        this.lineWidth = lineWidth;
        this.yVariation = yVariation;
        this.position = position;
        this.lightUV = lightUV;
        this.darkUV = darkUV;
        this.outlineUV = outlineUV;
        GenerateStripeAngle();
        GeneratePeak();
        GenerateContours();
    }

    private void GenerateStripeAngle ()
    {
        float angle = Random.value * Mathf.PI;
        sin = Mathf.Sin(angle);
        cos = Mathf.Cos(angle);
    }

    private void GeneratePeak ()
    {
        points = new Vector2[numVertices];

        for(int i = 0; i < numVertices; i++)
        {
            float xFrac = i == 0 ? 0 : (float)i / (numVertices - 1);
            float baseX = 2 * (xFrac- 0.5f);
            float x = baseX * width;

            float baseY = CalculateBaseHeight(baseX);
            baseY *= Random.value * yVariation + 1 - yVariation;
            float y = baseY * baseHeight;

            points[i] = new Vector2(x, y);

            if(points[i].y > points[peakIndex].y)
            {
                peakIndex = i;
            }
        }
    }

    private float CalculateBaseHeight (float baseX)
    {
        float oneMiunsX2 = 1 - (baseX * baseX);
        return oneMiunsX2 * oneMiunsX2 * oneMiunsX2;
    }    

    public Mesh GenerateMesh ()
    {
        Mesh mesh = new Mesh();
        vertices.Add(new Vector3(width / 2, 0, 0));
        vertices.Add(new Vector3(-width / 2, 0, 0));
        vertices.Add(new Vector3(0, width, 0));

        tris.Add(vertices.Count - 3);
        tris.Add(vertices.Count - 2);
        tris.Add(vertices.Count - 1);

        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0.5f, 1));


        //AddOutlinesToMesh();
        //AddContouredMesh();
        //AddContourLines();
        ApplyPsuedo3DEffect();
        ApplyPositionOffset();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, uvs);
        mesh.SetUVs(1, uvs.Select(v => (Vector2)position).ToList());
        return mesh;
    }

    private void ApplyPositionOffset ()
    {
        for(int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = vertices[i] + position;
        }
    }

    private void AddOutlinesToMesh ()
    {
        int baseIndex = vertices.Count;
        vertices.Add((Vector3)contours[peakContourIndex].EndPoint + new Vector3(0, 0, outlineZOffset));
        uvs.Add(outlineUV);

        for(int i = 1; i < points.Length -1; i++)
        {
            vertices.Add((Vector3)points[i]);
            if(i != 1 && i != points.Length - 2)
            {
                vertices[vertices.Count - 1] = vertices[vertices.Count - 1] * (1 + lineWidth / 3)
                    + new Vector3(0f, lineWidth, outlineZOffset);
            }
            uvs.Add(outlineUV);

            if(i > 0)
            {
                tris.Add(baseIndex);
                tris.Add(baseIndex + i);
                tris.Add(baseIndex + i + 1);
            }
        }        
    }   

    private void AddContourLines ()
    {
        for(int i = 0; i < contours.Count; i++)
        {
            vertices.Add((Vector3)points[contours[i].Index] + new Vector3(-lineWidth * 0.6f, lineWidth / 2, contourLineZOffset));
            vertices.Add((Vector3)points[contours[i].Index] + new Vector3(lineWidth * 0.6f, lineWidth / 2, contourLineZOffset));
            if(i % 2 == 0)
            {
                vertices.Add(contours[i].EndPoint);
            }
            else
            {
                if(i > peakContourIndex)
                {
                    vertices.Add(contours[i + 1].EndPoint);
                }
                else
                {
                    vertices.Add(contours[i - 1].EndPoint);
                }
            }

            vertices[vertices.Count - 1] += new Vector3(0, 0, contourLineZOffset);
            
            

            for(int j = 0; j < 3; j++)
            {
                uvs.Add(outlineUV);
            }

            tris.Add(vertices.Count - 3);
            tris.Add(vertices.Count - 2);
            tris.Add(vertices.Count - 1);
        }
    }

    private void AddContouredMesh ()
    {
        int baseIndex = vertices.Count;

        //Add contour endpoints
        contours.ForEach(c => vertices.Add((Vector3)c.EndPoint 
            + (c.Index == peakIndex ? Vector3.forward * contourBaseZOffset : Vector3.zero)));
        contours.ForEach(c => AddUV(c.Index >= peakIndex));

        //Second version of peak endpoint for different UVs
        vertices.Add((Vector3)contours[peakContourIndex].EndPoint + Vector3.forward * contourBaseZOffset);
        uvs.Add(lightUV);

        bool lastDark = true;
        bool dark;
        bool skipLast = false;

        //Add first vertex
        vertices.Add(new Vector2(points[1].x, points[0].y));
        AddUV(true);

        for(int i = 1; i < points.Length -1; i++)
        {
            dark = points[i].y > points[i - 1].y;

            if(dark != lastDark || (dark ^ i < contours[peakContourIndex].Index))
            {
                vertices.Add(points[i - 1]);
                AddUV(dark);            
            }
            vertices.Add(points[i]);
            AddUV(dark);

            int contourIndex = GetContourIndex(i, dark);
            if(contourIndex == peakContourIndex && !dark)
            {
                contourIndex = contours.Count;
            }
            AddNewTri(contourIndex + baseIndex, skipLast);
            if(!(i > peakIndex ^ dark) && contourIndex != peakIndex && contourIndex != contours.Count)
            {
                vertices.Add(points[i - 1]);
                vertices.Add(points[i]);
                AddUV(!dark);
                AddUV(!dark);
                AddNewTri(baseIndex + (!dark ? peakContourIndex : contours.Count));
            }
            lastDark = dark;

        }
    }

    private void AddUV (bool dark)
    {
        uvs.Add(dark ? darkUV : lightUV);
    }

    private void AddNewTri (int contourIndex, bool skipLast = false)
    {
        tris.Add(contourIndex);
        tris.Add(vertices.Count - 2 - (skipLast ? 2 : 0));
        tris.Add(vertices.Count - 1 - (skipLast ? 2 : 0));
    }

    private int GetContourIndex (int vertexIndex, bool dark)
    {
        if(dark && vertexIndex <= peakIndex || (!dark && vertexIndex >= peakIndex))
        {
            return peakContourIndex;
        }

        for(int i = contours.Count - 1; i > 0; i--)
        {
            if(vertexIndex > contours[i].Index - (vertexIndex > peakIndex ? 1 : 0))
            {
                return i;
            }
        }
        return 0;
    }

    private void ApplyPsuedo3DEffect ()
    {
        for(int i = 0; i < vertices.Count; i++)
        {
            vertices[i] -= new Vector3(0,0,vertices[i].y);
        }
    }

    private void GenerateContours ()
    {
        for(int i = 1; i < points.Length - 1; i++)
        {
            //If the peak changes direction at this vertex
            if((points[i - 1].y < points[i].y != points[i].y < points[i + 1].y) || Random.value < randomContourChance) 
            {
                List<Vector2> contour = new List<Vector2>();

                float xVariation = (Random.value * 2 - 1) * width / numVertices;
                float yPos = CalculateContourDepth(i);
                xVariation *= (points[i].y / points[peakIndex].y);

                contours.Add(new Contour(i, new Vector2(points[i].x + xVariation * 2, yPos)));

                if(i == peakIndex)
                {
                    peakContourIndex = contours.Count - 1;
                }
            }
        }
    }

    private float CalculateContourDepth (int pointIndex)
    {
        float peakHeight = points[peakIndex].y;
        float length = points[pointIndex].y / peakHeight;
        length *= length * peakHeight;

        //if(pointIndex == peakIndex) length *= 1.2f;

        return points[pointIndex].y - length;
    }

    public void DrawDebugLines (Vector2 position)
    {
        for(int i = 1; i < points.Length; i++)
        {
            Debug.DrawLine(position + points[i], position + points[i - 1]);
        }
        foreach(Contour contour in contours)
        {
            Debug.DrawLine(position + points[contour.Index], position + contour.EndPoint);
        }
    }
}


