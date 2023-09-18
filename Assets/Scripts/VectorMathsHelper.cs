using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class VectorMathsHelper
{

    public static float DistanceSquared(Vector3 pointA, Vector3 pointB)
    {
        Vector3 delta = pointA - pointB;
        return (delta.x * delta.x) + (delta.y * delta.y) + (delta.z * delta.z);
    }

    public static float Sqaure(float a)
    {
        return a * a;
    }

    public static float DistaneSqrBetweenSegmentAndPoint(Vector3 aPos, Vector3 bPos, Vector3 point)
    {
        float lengthSquared = DistanceSquared(aPos, bPos);
        //r = aPos + t (bPos-aPos)         
        float t = Mathf.Clamp01(Vector3.Dot(point - aPos, bPos - aPos) / lengthSquared);

        Vector3 projection = aPos + t * (bPos - aPos);

        float distSqr = DistanceSquared(point, projection);

        return distSqr;
    }

    public static CurveOrientationType GetCurveOrientation(Vector2 pos1, Vector2 pos2, Vector2 pos3)
    {
        float val = (pos2.y - pos1.y) * (pos3.x - pos2.x) -
                  (pos2.x - pos1.x) * (pos3.y - pos2.y);
        if (val == 0) return CurveOrientationType.COLINEAR;
        else return (val > 0) ? CurveOrientationType.CLOCKWISE : CurveOrientationType.ANTICLOCKWISE;
    }

    public static CurveOrientationType GetCurveOrientation(Vector3 pos1, Vector3 pos2, Vector3 pos3)
    {
        return GetCurveOrientation(Vector3ToVector2OnXZPlane(pos1), Vector3ToVector2OnXZPlane(pos2), Vector3ToVector2OnXZPlane(pos3));
    }

    //Checks if pos2 is on line pos1->pos3
    static bool IsPointInRect(Vector2 point, Vector2 rectPointA, Vector2 rectPointB)
    {
        return (point.x <= Mathf.Max(rectPointA.x, rectPointB.x)) && (point.x >= Mathf.Min(rectPointA.x, rectPointB.x)) &&
               (point.y <= Mathf.Max(rectPointA.y, rectPointB.y)) && (point.y >= Mathf.Min(rectPointA.y, rectPointB.y));
    }

    //Check if line segment pos1->pos2 intersects pos3->pos4
    public static bool DoSegmentsIntersect(Vector3 segment1PosA, Vector3 segment1PosB, Vector3 segment2PosA, Vector3 segment2PosB)
    {
        Vector2 seg1posA = new Vector2(segment1PosA.x, segment1PosA.z);
        Vector2 seg1posB = new Vector2(segment1PosB.x, segment1PosB.z);
        Vector2 seg2posA = new Vector2(segment2PosA.x, segment2PosA.z);
        Vector2 seg2posB = new Vector2(segment2PosB.x, segment2PosB.z);

        CurveOrientationType or1 = GetCurveOrientation(seg1posA, seg1posB, seg2posA);
        CurveOrientationType or2 = GetCurveOrientation(seg1posA, seg1posB, seg2posB);
        CurveOrientationType or3 = GetCurveOrientation(seg2posA, seg2posB, seg1posA);
        CurveOrientationType or4 = GetCurveOrientation(seg2posA, seg2posB, seg1posB);

        if (or1 != or2 && or3 != or4) return true;

        if (or1 == CurveOrientationType.COLINEAR && IsPointInRect(seg2posA, seg1posA, seg1posB)) return true;
        if (or2 == CurveOrientationType.COLINEAR && IsPointInRect(seg2posB, seg1posA, seg1posB)) return true;
        if (or3 == CurveOrientationType.COLINEAR && IsPointInRect(seg1posA, seg2posA, seg2posB)) return true;
        if (or4 == CurveOrientationType.COLINEAR && IsPointInRect(seg1posB, seg2posA, seg2posB)) return true;

        return false;
    }

    public static bool IsPointInPolygon(Vector3 point, IList<Vector3> verts)
    {
        int intersectingLines = 0;

        for (int i = 0; i < verts.Count; i++)
        {
            Vector3 vertA = verts[i];
            Vector3 vertB = verts[(i + 1) % verts.Count];

            bool pointToLeft = (point.x <= vertA.x) || (point.x <= vertB.x);

            if (pointToLeft)
            {
                bool pointAboveA = (point.z > vertA.z);
                bool pointAboveB = (point.z > vertB.z);
                bool pointInlineHorizontally = (point.z == vertA.z) || (point.z == vertB.z);

                // ^ is XOR

                if ((pointAboveA ^ pointAboveB) || pointInlineHorizontally)
                {
                    intersectingLines++;
                }
            }
        }
        //Odd number intersections
        if (intersectingLines % 2 == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static Vector2[] Vector3ArrayToVector2 (Vector3[] vec3Array)
    {
        return System.Array.ConvertAll(vec3Array, Vector3ToVector2);
    }

    public static Vector2[] Vector3ArrayToVector2OnXZPlane(Vector3[] vec3Array)
    {
        return System.Array.ConvertAll(vec3Array, Vector3ToVector2OnXZPlane);
    }

    public static Vector3[] Vector2ArrayToVector3(Vector2[] vec2Array)
    {
        return System.Array.ConvertAll(vec2Array, Vector2ToVector3);
    }

    public static Vector3 Vector2ToVector3 (Vector2 vec2)
    {
        return new Vector3(vec2.x, vec2.y);
    }

    public static Vector2 Vector3ToVector2OnXZPlane(Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.z);
    }

    public static Vector2 Vector3ToVector2(Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.y);
    }

}

public enum CurveOrientationType
{
    COLINEAR = 0,
    CLOCKWISE = 1,
    ANTICLOCKWISE = 2
}
