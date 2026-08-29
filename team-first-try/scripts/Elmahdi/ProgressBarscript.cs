using Godot;
using System;

public partial class ProgressBarscript : ProgressBar
{
	public void SetMaxScore(int maxScore)
    {
        MaxValue = maxScore;
    }

    public void UpdateScore(int currentScore)
    {
        Value = Mathf.Clamp(currentScore, 0, MaxValue);
    }
}
