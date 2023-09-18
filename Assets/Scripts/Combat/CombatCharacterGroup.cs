using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CombatCharacterGroup
{
    public enum Position
    {
        Center = 1,
        North = 2,
        East = 3,
        South = 4,
        West = 5,
        AllBorders = 6,
    }

    public Position Location { get; }

    public IEnumerable<Character> Characters { get; }
    public bool Friendly { get; }
    public bool PlayerControlled { get; }

    public CombatCharacterGroup (Position location, IEnumerable<Character> characters, bool friendly, bool playerControlled)
    {
        Location = location;
        Characters = characters;
        Friendly = friendly;
        PlayerControlled = playerControlled;
    }

    public void Spawn ()
    {
        var positions = GetPositions().OrderBy(p => Random.value).ToList();

        int i = 0;

        foreach(Character character in Characters)
        {
            CombatMap.Instance.SpawnAgent(positions[i], PlayerControlled, character, Friendly);
            i = (i + 1) % positions.Count;
        }
    }

    private IEnumerable<Vector2Int> GetPositions ()
    {
        int width = CombatMap.Instance.Grid.Width;
        int height = CombatMap.Instance.Grid.Height;
        int k = 4;

        switch(Location)
        {
            case Position.Center:
                for(int x = k; x < width - k; x++) 
                        for(int y = k; y < height - k; y++)            
                            yield return new Vector2Int(x, y);                        
                break;
            case Position.North:
                for(int x = 0; x < width; x++)
                {
                    yield return new Vector2Int(x, height - 1);
                }
                break;
            case Position.East:
                for(int y = 0; y < height; y++)
                {
                    yield return new Vector2Int(0, y);
                }
                break;
            case Position.South:
                for(int x = 0; x < width; x++)
                {
                    yield return new Vector2Int(x, 0);
                }
                break;
            case Position.West:
                for(int y = 0; y < height; y++)
                {
                    yield return new Vector2Int(width - 1, y);
                }
                break;
            case Position.AllBorders:
                for(int y = 0; y < height; y++)
                {
                    yield return new Vector2Int(0, y);
                    yield return new Vector2Int(width - 1, y);
                }
                for(int x = 0; x < width; x++)
                {
                    yield return new Vector2Int(x, 0);
                    yield return new Vector2Int(x, height - 1);
                }
                break;
            default:
                break;
        }
    }

}