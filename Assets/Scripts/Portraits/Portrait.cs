using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class Portrait
{
    public float SkullRoundness { get; set; } = 1;

    public float SkullSmoothness { get; set; } = 1;

    public Vector2 SkullSize { get; set; } = Vector2.one;

    public float JawRoundness { get; set; } = 1;

    public float JawSmoothness { get; set; } = 1;

    public Vector2 JawSize { get; set; } = Vector2.one;

    public float JawOffset { get; set; } = 1;

    public float ChinPoint { get; set; } = 1;

    public float NeckWidth { get; set; } = 1;

    public Vector2 EarSize { get; set; } = Vector2.one;

    public Vector2 NoseSize { get; set; } = Vector2.one;

    public float NoseProminence { get; set; } = 1;

    public float NostrilDefintion { get; set; } = 1;

    public float EyebrowCurveDepth { get; set; } = 1;
    public float EyebrowCurveOffset { get; set; } = 1;

    public float EyebrowThickness { get; set; } = 1;

    public float EyebrowHeight { get; set; } = 1;

    public float EyeSlant { get; set; } = 1;

    public Vector2 EyeSize { get; set; } = Vector2.one;

    public float UpperLidPeak { get; set; } = 1;
    public float UpperLidOffset { get; set; } = 1;
    public float LowerLidPeak { get; set; } = 1;
    public float LowerLidOffset { get; set; } = 1;

    public float LashThickness { get; set; } = 1;
    public float LashWingThickness { get; set; } = 1;

    public bool HasHair { get; set; } = true;

    public float HairLineHeight { get; set; } = 1;

    public float TopLipHeight { get; set; } = 1;
    public float BottomLipHeight { get; set; } = 1;

    public float MouthWidth { get; set; } = 1;

    public bool EyesOpen { get; set; } = true;

    public bool Infected { get; set; } = false;


    public Vector2 SkinColor { get; set; } = Vector2.zero;
    public Vector2 EyeColor { get; set; } = Vector2.zero;

    Mesh headMesh = new Mesh();
    List<Vector3> verts;
    List<int> tris;
    List<Vector2> uv;

    Mesh hairMesh = new Mesh();
    List<Vector3> hairVerts;
    List<int> hairTris;
    List<Vector2> hairUV;

    Mesh eyeMesh = new Mesh();
    List<Vector3> eyeVerts;
    List<int> eyeTris;
    List<Vector2> eyeUV;
    List<Vector2> eyeUV2;

    private readonly float sqrt2 = Mathf.Sqrt(2);
    private readonly float sqrtPi = Mathf.Sqrt(Mathf.PI);

    private Vector2 skinTone;
    private Vector2 eyeColor;
    private Vector2 SkinShadow { get { return skinTone + Vector2.up / 2; } }
    private Vector2 SkinBlush { get { return skinTone + Vector2.right / 2; } }
    private Vector2 SkinBlushShadow { get { return skinTone + Vector2.one / 2; } }

    private Vector2 boundsMin;
    private Vector2 boundsMax;

    private Vector2 HeadSize { get { return boundsMax - boundsMin; } }

    private readonly float hairBackZ = 0.05f;
    private readonly float earZ = 0.1f;
    private readonly float eyebrowZ = -0.3f;
    private readonly float eyeshadowZ = -0.1f;
    private readonly float eyeZ = -0.2f;

    private readonly int eyebrowVertices = 6;
    private readonly int skullVertices = 24;
    private readonly int noseVertices = 40;

    public void Generate ()
    {
        skinTone = SkinColor / 2;
        eyeColor = EyeColor;

        verts = new List<Vector3>();
        tris = new List<int>();
        uv = new List<Vector2>();

        hairVerts = new List<Vector3>();
        hairTris = new List<int>();
        hairUV = new List<Vector2>();

        eyeVerts = new List<Vector3>();
        eyeTris = new List<int>();
        eyeUV = new List<Vector2>();
        eyeUV2 = new List<Vector2>();

        verts.Add(Vector3.zero);
        hairVerts.Add(Vector3.forward * hairBackZ);
        uv.Add(skinTone);
        hairUV.Add(skinTone);

        for(int i = 0; i < skullVertices; i++)
        {
            Vector2 vert = GetHeadVertex(i);
            if(HasHair && i < skullVertices / 2)
            {
                float hairLine = (Mathf.Cos(vert.x * 2) + 1);

                verts.Add(vert * (1 - hairLine * 0.3f * (1 - HairLineHeight)));
            }
            else
            {
                verts.Add(vert);
            }

            tris.Add(i + 1);
            tris.Add(0);
            tris.Add((i + 1) % skullVertices + 1);
            uv.Add(skinTone);

            hairVerts.Add((Vector3)vert + Vector3.forward * hairBackZ);

            hairTris.Add(i + 1);
            hairTris.Add(0);
            hairTris.Add((i + 1) % skullVertices + 1);
            hairUV.Add(skinTone);
        }

        //Normalise head size
        NormaliseHeadSize();
        AddNeck();
        AddEar(leftEar: true);
        AddEar(leftEar: false);
        AddNose();
        AddLip(upper: true, SkinBlushShadow);
        AddLip(upper: false, SkinBlush);
        AddLip(upper: false, SkinShadow, true);
        AddEyebrow(true);
        AddEyebrow(false);
        AddEyeShadow(true);
        AddEyeShadow(false);
        if(EyesOpen)
        {
            AddEye(true);
            AddEye(false);
        }

        AddEyeLash(true);
        AddEyeLash(false);

        PopulateMesh(headMesh, verts, uv, tris);
        PopulateMesh(hairMesh, hairVerts, hairUV, hairTris);
        PopulateMesh(eyeMesh, eyeVerts, eyeUV, eyeTris);
    }

    public Mesh GetHeadMesh ()
    {
        return headMesh;
    }

    public Mesh GetHairMesh ()
    {
        return hairMesh;
    }

    public Mesh GetEyeMesh ()
    {        
        return eyeMesh;
    }

    private Mesh PopulateMesh (Mesh mesh, List<Vector3> verts, List<Vector2> uvs, List<int> tris)
    {
        mesh.Clear();
        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        if(mesh == eyeMesh)
        {
            mesh.SetUVs(1, eyeUV2);
        }
        mesh.RecalculateBounds();
        return mesh;
    }

    private (Vector2, Vector2) UpdateBounds ()
    {
        Vector2 min = Vector2.zero;
        Vector2 max = Vector2.zero;

        for(int i = 0; i < verts.Count; i++)
        {
            min = Vector2.Min(verts[i], min);
            max = Vector2.Max(verts[i], max);
        }

        boundsMin = min;
        boundsMax = max;

        return (min, max);
    }

    private void NormaliseHeadSize ()
    {
        var (min, max) = UpdateBounds();

        Vector2 size = max - min;
        float scale = 1 / size.magnitude;
        float mean = (max.y + min.y) / 2;

        for(int i = 0; i < verts.Count; i++)
        {
            verts[i] = new Vector3(verts[i].x * scale, (verts[i].y - mean) * scale, verts[i].z);
        }

        for(int i = 0; i < hairVerts.Count; i++)
        {
            hairVerts[i] = new Vector3(hairVerts[i].x * scale, (hairVerts[i].y - mean) * scale, hairVerts[i].z);
        }
        UpdateBounds();
    }

    private void AddNeck ()
    {
        float neckWidth = NeckWidth * (boundsMax.x - boundsMin.x);

        int baseIndex = verts.Count - (skullVertices / 2) - 1;
        verts.Add(Vector3.forward * .1f);
        int newZero = verts.Count - 1;
        uv.Add(skinTone + Vector2.up / 2);

        float maxY = -10000;

        for(int i = 3; i < skullVertices / 2; i++)
        {
            int index = baseIndex + i;

            Vector3 vert = verts[baseIndex + i];

            float y = Mathf.Max(vert.y / 1.5f -0.25f, -.6f);

            verts.Add(new Vector3(vert.x * NeckWidth, y, 0.1f));

            maxY = Mathf.Max(verts[verts.Count - 1].y, maxY);

            if(i > 1)
            {
                tris.Add(newZero);
                tris.Add(verts.Count - 1);
                tris.Add(verts.Count - 2);
            }

            uv.Add(skinTone + Vector2.up / 2);
        }

        AddQuad(new Vector3(boundsMin.x, 0, 0.1f),
                new Vector3(boundsMax.x, 0, 0.1f),
                verts[verts.Count - skullVertices /2 + 3],
                verts[verts.Count - 1],
                skinTone + Vector2.up / 2);


        AddQuad(new Vector3(-neckWidth / 2, 0, 0.12f),
                new Vector3(+neckWidth / 2, 0, 0.12f),
                new Vector3(-neckWidth / 1.9f, -.5f, 0.12f),
                new Vector3(+neckWidth / 1.9f, -.5f, 0.12f),
                skinTone);

        AddQuad(new Vector3(-neckWidth / 1.9f, -.5f, 0.12f),
        new Vector3(+neckWidth / 1.9f, -.5f, 0.12f),
        new Vector3(-neckWidth / 19f, -.6f, 0.12f),
        new Vector3(+neckWidth / 19f, -.6f, 0.12f),

        skinTone);
    }

            
    private void AddQuad (Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector2 uvCoord)
    {
        verts.Add(a);
        verts.Add(b);
        verts.Add(c);
        verts.Add(d);


        for(int i = 0; i < 4; i++)
        {
            uv.Add(uvCoord);
        }

        tris.Add(verts.Count - 4);
        tris.Add(verts.Count - 3);
        tris.Add(verts.Count - 2);

        tris.Add(verts.Count - 3);
        tris.Add(verts.Count - 1);
        tris.Add(verts.Count - 2);
    }

    private void AddEar (bool leftEar)
    {
        Vector2 min = boundsMin;
        Vector2 max = boundsMax;

        float multiplier = leftEar ? 1 : -1;

        float bottomEar = -(max.y - min.y) / 6;

        //Base - quad to join ear fully to head

        verts.Add(new Vector3((-max.x + .02f) * multiplier, 0.1f * EarSize.y, earZ));
        verts.Add(new Vector3((-max.x + 0.1f) * multiplier, 0.1f * EarSize.y, earZ));
        verts.Add(new Vector3((-max.x + .02f) * multiplier, bottomEar * EarSize.y, earZ));
        verts.Add(new Vector3((-max.x + 0.1f) * multiplier, bottomEar * EarSize.y, earZ));

        tris.Add(verts.Count - 4);
        tris.Add(verts.Count - (leftEar ? 3 : 2));
        tris.Add(verts.Count - (leftEar ? 2 : 3));

        tris.Add(verts.Count - 3);
        tris.Add(verts.Count - (leftEar ? 1 : 2));
        tris.Add(verts.Count - (leftEar ? 2 : 1));

        //Shell

        int centerVertIndex = verts.Count - 2;

        //Top
        verts.Add(new Vector3((-max.x - .03f * EarSize.x) * multiplier, 0.12f * EarSize.y, earZ));
        tris.Add(centerVertIndex);
        tris.Add(verts.Count - (leftEar ? 1 : 5));
        tris.Add(verts.Count - (leftEar ? 5 : 1));
        //Out up
        verts.Add(new Vector3((-max.x - .07f * EarSize.x) * multiplier, 0.08f * EarSize.y, earZ));
        //Out down
        verts.Add(new Vector3((-max.x - .055f * EarSize.x) * multiplier, (bottomEar + 0.05f) * EarSize.y, earZ));
        //Lobe
        verts.Add(new Vector3((-max.x - .02f * EarSize.x) * multiplier, (bottomEar - 0.02f) * EarSize.y, earZ));

        for(int i = 0; i < 3; i++)
        {
            int a = verts.Count - 1 - i;
            int b = verts.Count - 1 - ((i + 1) % 4);
            tris.Add(centerVertIndex);
            tris.Add(leftEar ? a : b);
            tris.Add(leftEar ? b : a);
        }

        for(int i = 0; i < 8; i++)
        {
            uv.Add(skinTone + Vector2.right / 2);
        }
    }

    private Vector2 GetHeadVertex (int index)
    {
        float offset = 0;// 1f / (skullVertices) * Mathf.PI;

        float angle = (float)index / (skullVertices) * Mathf.PI * 2 + offset;

        Vector2 circleVec = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        Vector2 squareVec = new Vector2(
            ClampMagnitude(circleVec.x * sqrt2, 1),
            ClampMagnitude(circleVec.y * sqrt2, 1));


        bool isJaw = index >= skullVertices / 2;

        Vector2 jawOffset = Vector2.up * JawOffset * 0.5f;
        float roundness = SkullRoundness;
        float smoothness = SkullSmoothness;
        Vector2 size = SkullSize;

        if(isJaw)
        {
            jawOffset *= -1;
            roundness = JawRoundness;
            smoothness = JawSmoothness;
            size = JawSize;

            float deltaChinAngle = 1 - Mathf.Clamp01(Mathf.Abs(angle - Mathf.PI * 3 / 2) / Mathf.PI * 3);


            size = new Vector2(size.x * (1 - ChinPoint * deltaChinAngle), size.y);

        }

        float curveFactor = smoothness + Vector2.Distance(circleVec, squareVec) / (sqrt2 - 1) * (1 - smoothness);

        Vector2 blended = Vector2.LerpUnclamped(squareVec, circleVec, roundness * curveFactor) * size + jawOffset;
        return blended;
    }

    private void AddNose ()
    {
        int baseIndex = verts.Count;

        Vector3 noseCenter = new Vector3(0, -HeadSize.y / 6 + 0.025f, -0.1f);

        verts.Add(noseCenter);
        uv.Add(SkinBlush);

        float definition = Mathf.Pow(100, NostrilDefintion);

        for(int i = 0; i < noseVertices; i++)
        {
            float angle = (float)i / (noseVertices) * Mathf.PI * 2;

            Vector2 circleVec = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            float noseHeight = (boundsMax.y) / 2;

            float prominence = Mathf.Max(Vector2.Dot(circleVec, Vector2.up), 0) * noseHeight * NoseProminence;

            float nostrilValue = Mathf.Abs(Vector2.Dot(circleVec, Vector2.left));
            nostrilValue = Mathf.Pow(nostrilValue, definition) * circleVec.x * (0.25f + 0.75f * NostrilDefintion);

            Vector2 modifier = new Vector2(nostrilValue * 0.1f, prominence) ;

            verts.Add((Vector3)((circleVec * 0.05f + modifier) * NoseSize) + noseCenter);
            tris.Add(baseIndex + i + 1);
            tris.Add(baseIndex);
            tris.Add(baseIndex + (i + 1) % noseVertices + 1);
            uv.Add(SkinBlush);
        }
    }

    private void AddLip (bool upper, Vector2 color, bool shadow = false)
    {
        Vector3 mouthCenter = new Vector3(0, boundsMin.y + HeadSize.y * 0.22f, (shadow ? -0.05f : -0.1f));

        float mouthWidth = HeadSize.x / 2;

        verts.Add(mouthCenter);
        uv.Add(color);

        int centerIndex = verts.Count - 1;

        for(int i = 0; i < 10; i++)
        {
            float proportion = ((float)i / 9 - 0.5f);
            float x = mouthWidth * proportion * MouthWidth;

            float y = (1 - proportion * proportion * 4) * 0.06f * (upper ? TopLipHeight : (shadow ? BottomLipHeight : Mathf.Lerp(BottomLipHeight, 0.6f, 0.5f)));

            verts.Add(mouthCenter + (Vector3)(new Vector2(x, y * (upper ? 1 : -1)) * (shadow ? new Vector3(0.8f, 1.5f) : Vector3.one)));
            uv.Add(color);

            if(i > 0)
            {
                tris.Add(centerIndex);
                tris.Add(verts.Count - (upper ? 2 : 1));
                tris.Add(verts.Count - (upper ? 1 : 2));
            }

        }
    }

    private float ClampMagnitude(float value, float max)
    {
        return Mathf.Min(Mathf.Abs(value), max) * Mathf.Sign(value);
    }


    private void AddEyebrow (bool left)
    {
        float eyebrowLeft = GetEyebrowStart();
        float eyebrowRight = GetEyebrowEnd();

        float center = GetEyebrowCenter();

        

        for(int i = 0; i < eyebrowVertices; i++)
        {
            float x = Mathf.Lerp(eyebrowLeft, eyebrowRight, (float)i / (eyebrowVertices -1));
            float y = GetEyebrowHeight(x, center);

            Vector3 bottom = new Vector3((left ? x : -x), y, eyebrowZ);
            Vector3 top = bottom + new Vector3(0, GetEyebrowThickness(x), 0);

            hairVerts.Add(bottom);
            hairVerts.Add(top);

            hairUV.Add(SkinShadow);
            hairUV.Add(SkinShadow);

            if(i > 0)
            {
                hairTris.Add(hairVerts.Count - (left ? 4 : 3));
                hairTris.Add(hairVerts.Count - (left ? 3 : 4));
                hairTris.Add(hairVerts.Count - 2);

                hairTris.Add(hairVerts.Count - (left ? 3 : 1));
                hairTris.Add(hairVerts.Count - (left ? 1 : 3));
                hairTris.Add(hairVerts.Count - 2);
            }
        }
    }

    private void AddEyeShadow (bool isLeft)
    {
        float center = GetEyePosition();
        float width = GetEyeSize();
        float left =  (-center - width / 2) * (isLeft ? 1 : -1);
        float right = (-center + width / 2) * (isLeft ? 1 : -1);

        int baseIndex = verts.Count;

        float eyebrowCenter = GetEyebrowCenter();

        verts.Add(new Vector3(right, GetEyeSlant(false), eyeshadowZ));
        uv.Add(SkinShadow);
        verts.Add(new Vector3(left, GetEyeSlant(true), eyeshadowZ));
        uv.Add(SkinShadow);

        float eyebrowStart = GetEyebrowStart();
        float eyebrowEnd = GetEyebrowEnd();

        for(int i = 0; i < eyebrowVertices; i++)
        {
            float x = Mathf.Lerp(eyebrowStart, eyebrowEnd, (float)i / (eyebrowVertices - 1));
            float y = GetEyebrowHeight(x, eyebrowCenter);

            Vector3 bottom = new Vector3(isLeft ? x : - x, y, eyeshadowZ);

            verts.Add(bottom);
            uv.Add(SkinShadow);

            tris.Add(baseIndex);
            tris.Add(verts.Count - (isLeft ? 2 : 1));
            tris.Add(verts.Count - (isLeft ? 1 : 2));
        }
    }

    private float GetEyebrowStart ()
    {
        return boundsMin.x + HeadSize.x / 9;
    }

    private float GetEyebrowEnd ()
    {
        return GetEyebrowStart() + HeadSize.x / 3;
    }

    private float GetEyebrowCenter ()
    {
        return Mathf.Lerp(GetEyebrowStart(), GetEyebrowEnd(), EyebrowCurveOffset);
    }

    private float GetEyebrowHeight (float x, float center)
    {
        float relativeX = 6 * (Mathf.Abs(x) + center) / HeadSize.x;

        float curve = (1 - (relativeX * relativeX)) * 0.1f * EyebrowCurveDepth;

        float dampenedCurve = curve / (1 + 6 * Mathf.Abs(EyebrowCurveOffset - 0.5f));

        return dampenedCurve + HeadSize.y / 6 - 0.08f * (1-EyebrowHeight);
    }

    private float GetEyebrowThickness (float x)
    {
        float relativeX = 2 * (Mathf.Abs(x)) / HeadSize.x;
        return Mathf.Sqrt(1 - relativeX) * 0.15f * EyebrowThickness;
    }

    private float GetEyePosition ()
    {
        return HeadSize.x / 4;
    }

    private float GetEyeSize ()
    {
        return HeadSize.x / 3 * EyeSize.x;
    }

    private float GetEyeSlant (bool outer)
    {
        return 0.025f * (EyeSlant - 0.25f) * (outer ? 1 : -1);
    }

    private float GetLidDistance (float xProportion, bool upper)
    {
        float offset = upper ? UpperLidOffset : LowerLidOffset;

        float x2 = offset > 0.5f ? xProportion : (1 - xProportion);

        float k = 1 - Mathf.Abs(0.5f - offset) * 1.6f;

        float m = Mathf.Pow(x2, k);

        float n = (m - 0.5f);

        float curve = 1 - 4 * n * n;

        return curve * EyeSize.y * (upper ? UpperLidPeak : LowerLidPeak) * 0.05f; 
    }


    private void AddEye (bool left)
    {
        float eyePos = GetEyePosition();
        float eyeWidth = GetEyeSize();
        Vector2 eyeStart = new Vector2(-eyePos - eyeWidth / 2, GetEyeSlant(true)) ;
        Vector2 eyeEnd = new Vector2(-eyePos + eyeWidth / 2, GetEyeSlant(false));

        Vector3 center = new Vector3(left ? -eyePos : eyePos, (eyeStart.y + eyeEnd.y) / 2, eyeZ);

        for(int i = 0; i < eyebrowVertices; i++)
        {
            float xProportion = (float)i / (eyebrowVertices - 1);

            Vector2 baseline = Vector2.Lerp(eyeStart, eyeEnd, xProportion);


            float topY = GetLidDistance(xProportion, true);
            float bottomY = GetLidDistance(xProportion, false);

            Vector3 basePos = baseline * new Vector3(left ? 1 : -1, 1, 1);

            Vector3 bottom = basePos + new Vector3(0, -bottomY, eyeZ);
            Vector3 top = basePos + new Vector3(0, topY, eyeZ);

            eyeVerts.Add(bottom);
            eyeVerts.Add(top);

            Vector2 deltaBottom = bottom - center;
            Vector2 deltaTop = top - center;

            Vector2 pupilOffset = new Vector2((left ? -1 : 1) * EyeSize.x * 0.04f, -0.05f);

            eyeUV.Add(Vector2.one / 2 + (Infected ? Vector2.zero : deltaBottom / EyeSize.x * 4 + pupilOffset));
            eyeUV.Add(Vector2.one / 2 + (Infected ? Vector2.zero : deltaTop / EyeSize.x * 4 + pupilOffset));

            eyeUV2.Add(eyeColor);
            eyeUV2.Add(eyeColor);

            if(i > 0)
            {
                eyeTris.Add(eyeVerts.Count - (left ? 4 : 3));
                eyeTris.Add(eyeVerts.Count - (left ? 3 : 4));
                eyeTris.Add(eyeVerts.Count - 2);
   
                eyeTris.Add(eyeVerts.Count - (left ? 3 : 1));
                eyeTris.Add(eyeVerts.Count - (left ? 1 : 3));
                eyeTris.Add(eyeVerts.Count - 2);
            }
        }
    }

    private void AddEyeLash (bool left)
    {
        float eyePos = GetEyePosition();
        float eyeWidth = GetEyeSize();
        Vector2 eyeStart = new Vector2(-eyePos - eyeWidth / 2, GetEyeSlant(true));
        Vector2 eyeEnd = new Vector2(-eyePos + eyeWidth / 2, GetEyeSlant(false));

        for(int i = 0; i < eyebrowVertices; i++)
        {
            float xProportion = (float)i / (eyebrowVertices - 1);

            Vector2 baseline = Vector2.Lerp(eyeStart, eyeEnd, xProportion);

            float eyeTopY = GetLidDistance(xProportion, EyesOpen) * (EyesOpen ? 1 : -1);

            Vector3 basePos = baseline * new Vector3(left ? 1 : -1, 1, 1);

            Vector3 eyeTop = basePos + new Vector3(0, eyeTopY, eyeZ);

            float thickness = Mathf.Lerp(LashWingThickness, Mathf.Lerp(LashThickness, 0, xProportion - 0.5f), xProportion + 0.5f);

            Vector3 wing = new Vector3(Mathf.Clamp01(0.5f - xProportion) * LashWingThickness * 0.1f * (left ? -1 : 1), 0);

            eyeVerts.Add(eyeTop);
            eyeVerts.Add(eyeTop + Vector3.up * thickness * 0.06f * (EyesOpen ? 1 : -1) + wing);

            


            eyeUV.Add(Vector2.one / 2);
            eyeUV.Add(Vector2.one / 2);
            eyeUV2.Add(eyeColor);
            eyeUV2.Add(eyeColor);

            if(i > 0)
            {
                eyeTris.Add(eyeVerts.Count - (left ^ !EyesOpen ? 4 : 3));
                eyeTris.Add(eyeVerts.Count - (left ^ !EyesOpen ? 3 : 4));
                eyeTris.Add(eyeVerts.Count - 2);

                eyeTris.Add(eyeVerts.Count - (left ^ !EyesOpen ? 3 : 1));
                eyeTris.Add(eyeVerts.Count - (left ^ !EyesOpen ? 1 : 3));
                eyeTris.Add(eyeVerts.Count - 2);
            }
        }
    }




}
