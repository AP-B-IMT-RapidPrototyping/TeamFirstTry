using Godot;
using System;

public partial class PauseScreenScript : Control
{
	private Button _resumeButton;
    private Button _exitButton;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        _resumeButton = GetNode<Button>("VBoxContainer/ResumeButton");
        _exitButton = GetNode<Button>("VBoxContainer/ExitButton");

        _resumeButton.Pressed += OnResumePressed;
        _exitButton.Pressed += OnExitPressed;

        Visible = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel") && !@event.IsEcho() && @event.IsPressed())
        {
            TogglePause();
            GetViewport().SetInputAsHandled();
        }
    }

    private void TogglePause()
    {
        bool newState = !GetTree().Paused;

    	GetTree().Paused = newState;
    	Visible = newState;

    	Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    private void OnResumePressed()
    {
        TogglePause();
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}
