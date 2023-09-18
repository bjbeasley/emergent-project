using UnityEngine;

public class AICell
{
    public ActionTarget BestTarget { get; set; }
    public float ActionScore { get; set; }

    public float PositionScore { get; set; }


    public AICell ()
    {
        Clear();
    }


    public void Clear ()
    {
        BestTarget = null;
        ActionScore = 0;
        PositionScore = 0;
    }

    public float GetScore (float offensiveness, float fuzzing = 0)
    {
        return ActionScore * offensiveness + PositionScore + Random.value * 0.001f * fuzzing;
    }

}
