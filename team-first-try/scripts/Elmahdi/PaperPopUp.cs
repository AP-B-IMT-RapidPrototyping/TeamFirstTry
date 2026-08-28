using Godot;
using System;

public partial class PaperPopUp : CanvasLayer
{
    [Signal]
    public delegate void PaperOpenedEventHandler();

    [Signal]
    public delegate void PaperClosedEventHandler();

	private Label _popUpText;
    [Export] public Button _exitButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_popUpText = GetNode<Label>("PaperPanel/DocumentText");
        _exitButton = GetNode<Button>("PaperPanel/Button");

        _exitButton.Pressed += OnCloseButtonPressed;
		Visible = false;
	}

	public void DisplayNote(string content)
    {
        _popUpText.Text = content;
        Visible = true;

        Input.MouseMode = Input.MouseModeEnum.Visible;
        EmitSignal(SignalName.PaperOpened);
    }

    private void OnCloseButtonPressed()
    {
        Visible = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;

        EmitSignal(SignalName.PaperClosed);
    }
}
