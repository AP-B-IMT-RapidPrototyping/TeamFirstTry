using Godot;
using System;

public partial class NPC : Node3D
{
    [Export] private PaperPopUp _paperpopup;
    
    private bool _isPlayerInZone = false;

    public override void _Ready()
    {
        Area3D interactionArea = GetNode<Area3D>("InteractionArea");

        interactionArea.BodyEntered += OnBodyEntered;
        interactionArea.BodyExited += OnBodyExited;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_isPlayerInZone && @event.IsActionPressed("interact"))
        {
            _paperpopup?.DisplayNote("Hello! How can I help you?");
        }
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is CharacterBody3d) 
        {
            _isPlayerInZone = true;
        }
    }

    private void OnBodyExited(Node3D body)
    {
        if (body is CharacterBody3d)
        {
            _isPlayerInZone = false;
        }
    }
}
