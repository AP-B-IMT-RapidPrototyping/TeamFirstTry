using Godot;
using System;

public partial class CharacterBody3d : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	[Export] private PaperPopUp _popUpText;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			if (keyEvent.Keycode == Key.I)
			{
				_popUpText?.DisplayNote("Hello World! This is a test.");
			}
		}
	}



}
