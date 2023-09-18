using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using EmpiresCore;
using Random = System.Random;

class ProvinceSampler
{
    private Random random;
    private int[,] backgroundGrid;
    private readonly float cellSize;
    private readonly float minDistanceSqr;
    private readonly Province province;

    private float width;
    private float height;
    private Vec2 offset;

    private float xBorderDistance;
    private float yBorderDistance;

    private readonly List<Vec2> samples = new List<Vec2>();

    public ProvinceSampler (Province province, float minDistance, float xBorderDistance, float yBorderDistance)
    {
        random = new Random(province.ID);
        this.province = province;
        minDistanceSqr = minDistance * minDistance;
        cellSize = minDistance / (float)Math.Sqrt(2);

        this.xBorderDistance = xBorderDistance;
        this.yBorderDistance = yBorderDistance;

        CalculateDimensions();

        InitialiseBackgroundGrid();
    }

    private void CalculateDimensions ()
    {
        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;

        float minY = float.PositiveInfinity;
        float maxY = float.NegativeInfinity;

        foreach(Vec2 vertex in province.GetVertices(true))
        {
            minX = Mathf.Min(minX, vertex.X);
            maxX = Mathf.Max(maxX, vertex.X);

            minY = Mathf.Min(minY, vertex.Y);
            maxY = Mathf.Max(maxY, vertex.Y);
        }

        width = maxX - minX;
        height = maxY - minY;

        offset = new Vec2(minX, minY);
    }

    private void InitialiseBackgroundGrid ()
    {
        int horizontalCells = (int)Math.Ceiling(width / cellSize);
        int verticalCells = (int)Math.Ceiling(height / cellSize);

        backgroundGrid = new int[horizontalCells, verticalCells];

        for(int i = 0; i < horizontalCells; i++)
        {
            for(int j = 0; j < verticalCells; j++)
            {
                backgroundGrid[i, j] = -1;
            }
        }
    }

    public List<Vec2> AttemptToFindSamples (int attempts)
    {
        for(int i = 0; i < attempts; i++)
        {
            AttemptToFindNextSample();
        }
        return samples;
    }

    void AttemptToFindNextSample ()
    {
        Vec2 newSample = GenerateSample();

        if(CheckSample(newSample))
        {
            AddSample(newSample);
        }
    }

    Vec2 GenerateSample ()
    {
        Vec2 sampleFractions = new Vec2((float)random.NextDouble(), (float)random.NextDouble());

        return new Vec2(sampleFractions.X * width, sampleFractions.Y * height) + offset;
    }

    private void AddSample (Vec2 sample)
    {
        samples.Add(sample);

        Vector2Int gridIndices = GetGridIndices(sample);

        backgroundGrid[gridIndices.x, gridIndices.y] = samples.Count - 1;
    }

    private Vector2Int GetGridIndices (Vec2 sample)
    {
        sample -= offset;
        int x = (int)(sample.X / cellSize);
        int y = (int)(sample.Y / cellSize);

        return new Vector2Int(x, y);
    }

    bool CheckSample (Vec2 newSample)
    {
        if(newSample.Y < offset.X || newSample.Y > offset.Y + height || float.IsNaN(newSample.Y))
        {
            return false;
        }

        Vector2Int gridIndices = GetGridIndices(newSample);

        int minX = gridIndices.x > 1 ? gridIndices.x - 1 : 0;
        int minY = gridIndices.y > 1 ? gridIndices.y - 1 : 0;

        int gridWidth = backgroundGrid.GetLength(0);
        int gridHeight = backgroundGrid.GetLength(1);

        int maxX = gridIndices.x < gridWidth - 1 ? gridIndices.x + 1 : gridWidth - 1;
        int maxY = gridIndices.y < gridHeight - 1 ? gridIndices.y + 1 : gridHeight - 1;

        for(int i = minX; i <= maxX; i++)
        {
            for(int j = minY; j <= maxY; j++)
            {
                int sampleIndex = backgroundGrid[i, j];

                if(sampleIndex != -1)
                {
                    Vec2 otherSample = samples[sampleIndex];
                    float distanceSqr = Vec2.DistanceSquared(newSample, otherSample);

                    if(distanceSqr < minDistanceSqr)
                    {
                        return false;
                    }
                }
            }
        }

        return CheckBorders(newSample);
    }

    private bool CheckBorders (Vec2 sample)
    {
        if(!province.PointInsideProvince(sample))
        {
            return false;
        }
        if(xBorderDistance > 0)
        {
            for(int i = -1; i <= 1; i+=2)
            {
                if(!province.PointInsideProvince(sample + new Vec2(xBorderDistance * i, 0)))
                {
                    return false;
                }
            }
        }
        if(yBorderDistance > 0)
        {
            for(int i = -1; i <= 1; i += 2)
            {
                if(!province.PointInsideProvince(sample + new Vec2(0, yBorderDistance * i)))
                {
                    return false;
                }
            }
        }
        return true;
    }



}

