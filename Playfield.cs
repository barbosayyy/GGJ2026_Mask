using Godot;
using System;
using System.Collections.Generic;

public partial class Playfield : Node3D
{
	List<Node3D> planeMeshes;

	int planeWidth = 100;
	int planeHeight = 100;

	PlayerController player;

	[Export] Vector3 planeScale = new Vector3(8,8,8);
	[Export] Vector3 planeInitOffset = new Vector3(-16,0,-16f);
	[Export] Vector3 spacing = new Vector3(16,0,16);

	Vector3 moveToPos;

	[Export]
	public PackedScene ClickPulseScene;

	public override void _Ready()
	{
		player = (PlayerController)FindChild("Player", true, false);
		Vector3 initPos = new Vector3(planeInitOffset.X, 0, planeInitOffset.Z);

		for (int i = 0; i <= 2; i++)
		{
			for (int j = 0; j <= 2; j++)
			{
				MeshInstance3D planeMesh = new MeshInstance3D();
				planeMesh.Mesh = new PlaneMesh();
				planeMesh.Scale = new Vector3(planeScale.X, planeScale.Y, planeScale.Z);

				StaticBody3D staticBody = new StaticBody3D();
				staticBody.CollisionLayer = 2;
				staticBody.InputRayPickable = true;
				staticBody.InputEvent += OnInput;
				staticBody.Position = new Vector3(initPos.X + spacing.X * j, initPos.Y, initPos.Z + spacing.Z * i);

				CollisionShape3D collisionShape = new CollisionShape3D();
				collisionShape.Shape = new BoxShape3D()
				{
					Size = new Vector3(2, 0.01f, 2)
				};
				collisionShape.Scale = new Vector3(planeMesh.Scale.X, 1, planeMesh.Scale.Z);

				staticBody.AddChild(collisionShape);
				staticBody.AddChild(planeMesh, false);

				AddChild(staticBody);
			}
		}
	}

	public override void _Process(double delta)
	{
	}

	bool isPressed = false;

	private void OnInput(Node camera, InputEvent @event, Vector3 position, Vector3 normal, long shapeIdx)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
		{
			// Send moveto signal to player
			//GD.Print($"Clicked plane at position: {position}");
			player.moveToPos = position;
			isPressed = true;
			
			MeshInstance3D pulse = (MeshInstance3D)ClickPulseScene.Instantiate();
			pulse.Position = new Vector3(position.X, 0, position.Z);
			AddChild(pulse);
		}
		else if (@event is InputEventMouseButton mEvent && mEvent.ButtonIndex == MouseButton.Right && mEvent.IsPressed() == false)
		{
			// GD.Print($"Released at position: {position}");
			player.moveToPos = position;
			isPressed = false;
		}
		else if (@event is InputEventMouseMotion motionEvent && isPressed)
		{
			// When dragging, update position
			// GD.Print($"Motion at: {position}");
			player.moveToPos = position;
		}
	}
}
