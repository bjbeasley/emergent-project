using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TransformGridTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void TransformGridSimpleCoordinateConversion()
    {
        TransformGrid<bool> grid = new TransformGrid<bool>(Vector3.zero, Vector3.right, Vector3.up, 10, 10);

        for(int i = 0; i < 10; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                Vector2Int gridPos = new Vector2Int(i, j);
                Vector3 worldPos = grid.GridToWorldPos(gridPos);

                Assert.AreEqual(gridPos, grid.WorldToGridPosInt(worldPos));
            }
        }
    }

    [Test]
    public void TransformGridRandomCoordinateConversion ()
    {
        TransformGrid<bool> grid = new TransformGrid<bool>(Vector3.zero, Vector3.right, Vector3.up, 10, 10);

        for(int i = 0; i < 10; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                Vector2Int gridPos = new Vector2Int(i, j);
                Vector3 worldPos = grid.GridToWorldPos(gridPos);
                worldPos += new Vector3(Random.value * .99f - 0.5f, Random.value * .99f - 0.5f, Random.value * 1000);

                Assert.AreEqual(gridPos, grid.WorldToGridPosInt(worldPos));
            }
        }
    }

    [Test]
    public void TransformGridAltCoordinateConversion ()
    {
        TransformGrid<bool> grid = new TransformGrid<bool>(new Vector3(100,0, 6), new Vector3(3,0, 0), Vector3.up, 101, 10);

        for(int i = 0; i < 101; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                Vector2Int gridPos = new Vector2Int(i, j);
                Vector3 worldPos = grid.GridToWorldPos(gridPos);

                Assert.AreEqual(gridPos, grid.WorldToGridPosInt(worldPos));
            }
        }
    }



    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TransformGridTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
