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

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
    {
        // 2. Check if the 'I' key was pressed on the keyboard
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
        {
            if (keyEvent.Keycode == Key.I)
            {
                // 3. Trigger your paper test
                _popUpText?.DisplayNote("Hello World! This is a test.");
            }
        }
    }



}
