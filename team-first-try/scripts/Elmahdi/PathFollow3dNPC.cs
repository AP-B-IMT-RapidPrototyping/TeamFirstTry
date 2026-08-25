using Godot;
using System;

public partial class PathFollow3dNPC : PathFollow3D
{

	[Export] private float _speed = 3.0f;
	

	public override void _Process(double delta)
	{
		Progress += _speed * (float)delta;
	}
}
