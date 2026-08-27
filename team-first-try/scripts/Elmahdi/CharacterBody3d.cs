using Godot;
using System;

public partial class CharacterBody3d : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	[Export] public float MouseSensitivity = 0.003f;
	[Export] public float MinPitch = -89.0f;
	[Export] public float MaxPitch = 89.0f;
	[Export] private PaperPopUp _popUpText;

	private Node3D _head;
	private float _cameraRotationX = 0.0f;

	public override void _Ready()
	{
    	_head = GetNode<Node3D>("Head");
    	Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
    	if (@event is InputEventMouseMotion mouseMotion)
    	{
        	RotateY(-mouseMotion.Relative.X * MouseSensitivity);

        	_cameraRotationX -= mouseMotion.Relative.Y * MouseSensitivity;
        	_cameraRotationX = Mathf.Clamp(_cameraRotationX, Mathf.DegToRad(MinPitch), Mathf.DegToRad(MaxPitch));

        	Vector3 currentRotation = _head.Rotation;
        	currentRotation.X = _cameraRotationX;
        	_head.Rotation = currentRotation;
    	}

    	if (@event.IsActionPressed("ui_cancel"))
    	{
        	Input.MouseMode = Input.MouseModeEnum.Visible;
    	}
    	else if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
    	{
        	if (Input.MouseMode == Input.MouseModeEnum.Visible)
        	{
            	Input.MouseMode = Input.MouseModeEnum.Captured;
        	}
    	}

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
