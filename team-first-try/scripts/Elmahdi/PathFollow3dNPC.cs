using Godot;
using System;

public partial class PathFollow3dNPC : PathFollow3D
{
    [Export] private float _speed = 3.0f;
    [Export] private PackedScene _npcScene;

    private Node3D _currentNpc = null;

    public override void _Process(double delta)
    {
        // Move the NPC along the path until it reaches the end
        if (_currentNpc != null && ProgressRatio < 1.0f)
        {
            Progress += _speed * (float)delta;
        }
    }

    // Changed to public so DeskManager can call it
    public void SpawnNpc()
    {
        if (_currentNpc != null || _npcScene == null) 
            return;

        _currentNpc = _npcScene.Instantiate<Node3D>();
        AddChild(_currentNpc);
        Progress = 0.0f; // Start at the beginning of the path
    }
    
    // I Changed it to public so DeskManager can call it
    public void DeleteNpc()
    {
        if (_currentNpc == null) 
            return;

        _currentNpc.QueueFree();
        _currentNpc = null;
    }
}