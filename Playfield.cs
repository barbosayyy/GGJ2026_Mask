using Godot;
using System;
using System.Collections.Generic;

public partial class Playfield : Node3D
{
	List<Node3D> planeMeshes;

	int planeWidth = 100;
	int planeHeight = 100;

	PlayerController player;

	[Export] Vector3 planeScale;
	[Export] Vector3 planeInitOffset;
	[Export] Vector3 spacing;

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

	private void OnInput(Node camera, InputEvent @event, Vector3 position, Vector3 normal, long shapeIdx)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
		{
			GD.Print($"Clicked plane at position: {position}");
			player.moveToPos = new Vector3(position.X, 0, position.Z);

			MeshInstance3D pulse = (MeshInstance3D)ClickPulseScene.Instantiate();
			GD.Print($"here");
			pulse.Position = new Vector3(position.X, 0, position.Z);
			AddChild(pulse);
		}
	}
}
