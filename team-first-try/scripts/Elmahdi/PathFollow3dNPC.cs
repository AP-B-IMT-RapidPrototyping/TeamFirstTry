using Godot;
using System;

public partial class PathFollow3dNPC : PathFollow3D
{

	[Export] private float _speed = 3.0f;
	[Export] private PackedScene _npcScene;

	private Node3D _currentNpc = null;

	public override void _Process(double delta)
	{
		if (Input.IsKeyPressed(Key.H))
        {
            SpawnNpc();
        }
        else if (Input.IsKeyPressed(Key.J))
        {
            DeleteNpc();
        }

		if (_currentNpc != null)
        {
            Progress += _speed * (float)delta;
        }
		}
		private void SpawnNpc()
    {

        if (_currentNpc != null || _npcScene == null) 
            return;

        _currentNpc = _npcScene.Instantiate<Node3D>();
        AddChild(_currentNpc);
        Progress = 0.0f; 
    	}
		
		private void DeleteNpc()
    	{
        if (_currentNpc == null) 
            return;

        _currentNpc.QueueFree();
        _currentNpc = null;
    	}
}

