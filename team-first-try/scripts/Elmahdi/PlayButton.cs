using Godot;
using System;

public partial class PlayButton : Button
{
	
	public string TargetScenePath = "res://scenes/ElMahdi/Level1.tscn";

	public override void _Ready()
	{
		Pressed += OnButtonPressed;
	}

	private void OnButtonPressed()
    {
        GetTree().ChangeSceneToFile(TargetScenePath);
    }
}
