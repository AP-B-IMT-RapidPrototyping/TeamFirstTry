using Godot;
using System;

public partial class PaperPopUp : CanvasLayer
{
	private Label _popUpText;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_popUpText = GetNode<Label>("PaperPanel/DocumentText");

		Visible = false;
	}

	public void DisplayNote(string content)
    {
        _popUpText.Text = content;
        Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    // Connect this to your Close Button's "pressed" signal in the Inspector
    private void OnCloseButtonPressed()
    {
        Visible = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
}
