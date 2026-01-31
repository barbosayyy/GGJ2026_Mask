using Godot;
using System;

public partial class PlayerController : Node3D
{
	Camera3D cam;

	Vector3 camPos;
	Vector3 currentPos;
	public Vector3 moveToPos;
	[Export]float velocity;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentPos = Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		currentPos += new Vector3(Position.DirectionTo(moveToPos).X * velocity * (float)delta, 0, Position.DirectionTo(moveToPos).Z * velocity * (float)delta);
		Position = currentPos;
	}
}
