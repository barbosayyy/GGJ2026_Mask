using Godot;
using System;
using System.Collections.Generic;

public partial class Playfield : Node3D
{

	List<StaticBody3D> bodyList = new List<StaticBody3D>();

	int planeWidth = 100;
	int planeHeight = 100;


	PlayerController player;

	[Export] Vector3 planeScale = new Vector3(32,32,32);
	[Export] Vector3 planeInitOffset = new Vector3(-64,0,-64f);
	[Export] Vector3 spacing = new Vector3(64,0,64);
	float defaultLoadDistanceX = 16;
	float defaultLoadDistanceZ = 16;

	float loadDistanceX;
	float loadDistanceZ;

	Vector3 moveToPos;

	[Export]
	public PackedScene ClickPulseScene;	

	StaticBody3D temp0 = new StaticBody3D();
	StaticBody3D temp1 = new StaticBody3D();
	StaticBody3D temp2 = new StaticBody3D();

	[Export] Material grassMat; 

	// Assign this script to the playable scene

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = (PlayerController)FindChild("Player",true,false);
		Vector3 initPos = new Vector3(planeInitOffset.X, 0, planeInitOffset.Z);

		MeshInstance3D test = new MeshInstance3D();

		// player.AddChild(test)

		loadDistanceX = defaultLoadDistanceX;
		loadDistanceZ = defaultLoadDistanceZ;

		for(int i = 0; i <= 2; i++)
		{
			for(int j = 0; j <= 2; j++)
			{
				MeshInstance3D planeMesh = new MeshInstance3D();
				planeMesh.Mesh = new PlaneMesh();
				planeMesh.Scale = new Vector3(planeScale.X,planeScale.Y,planeScale.Z);
				planeMesh.MaterialOverride = grassMat;

				StaticBody3D staticBody = new StaticBody3D();
				staticBody.CollisionLayer = 1;
				staticBody.InputRayPickable = true;
				staticBody.InputEvent += OnInput;
				staticBody.Position = new Vector3(initPos.X+spacing.X*j, initPos.Y, initPos.Z+spacing.Z*i);
				CollisionShape3D collisionShape = new CollisionShape3D();
				collisionShape.Shape = new BoxShape3D()
				{
					Size = new Vector3(2, 0.01f, 2)  // Match your plane size
				};
				collisionShape.Scale = new Vector3(planeMesh.Scale.X, 1, planeMesh.Scale.Z);
				staticBody.AddChild(collisionShape);
				staticBody.AddChild(planeMesh, false);

				AddChild(staticBody);
				bodyList.Add(staticBody);
				GD.Print("Added");
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector3 tempX = new Vector3(0,0,player.Position.Z);
		Vector3 tempZ = new Vector3(player.Position.X,0,0);
		tempX.X = bodyList[2].Position.X;
		if(player.Position.DistanceTo(tempX) <= 8 )
		{
			bodyList[0].Position += new Vector3(spacing.X, 0, 0);
			bodyList[1].Position += new Vector3(spacing.X, 0, 0);
			bodyList[2].Position += new Vector3(spacing.X, 0, 0);
			bodyList[3].Position += new Vector3(spacing.X, 0, 0);
			bodyList[4].Position += new Vector3(spacing.X, 0, 0);
			bodyList[5].Position += new Vector3(spacing.X, 0, 0);
			bodyList[6].Position += new Vector3(spacing.X, 0, 0);
			bodyList[7].Position += new Vector3(spacing.X, 0, 0);
			bodyList[8].Position += new Vector3(spacing.X, 0, 0);
		}
		tempX.X = bodyList[0].Position.X;
		if(player.Position.DistanceTo(tempX) <= 8)
		{
			bodyList[0].Position -= new Vector3(spacing.X, 0, 0);
			bodyList[1].Position -= new Vector3(spacing.X, 0, 0);
			bodyList[2].Position -= new Vector3(spacing.X, 0, 0);
			bodyList[3].Position -= new Vector3(spacing.X, 0, 0);
			bodyList[4].Position -= new Vector3(spacing.X, 0, 0);
			bodyList[5].Position -= new Vector3(spacing.X, 0, 0);
			bodyList[6].Position -= new Vector3(spacing.X, 0, 0);
			bodyList[7].Position -= new Vector3(spacing.X, 0, 0);
			bodyList[8].Position -= new Vector3(spacing.X, 0, 0);
		}
		tempZ.Z = bodyList[0].Position.Z;
		if(player.Position.DistanceTo(tempZ) <= 8)
		{
			bodyList[0].Position -= new Vector3(0, 0, spacing.Z);
			bodyList[1].Position -= new Vector3(0, 0, spacing.Z);
			bodyList[2].Position -= new Vector3(0, 0, spacing.Z);
			bodyList[3].Position -= new Vector3(0, 0, spacing.Z);
			bodyList[4].Position -= new Vector3(0, 0, spacing.Z);
			bodyList[5].Position -= new Vector3(0, 0, spacing.Z);
			bodyList[6].Position -= new Vector3(0, 0, spacing.Z);
			bodyList[7].Position -= new Vector3(0, 0, spacing.Z);
			bodyList[8].Position -= new Vector3(0, 0, spacing.Z);

			loadDistanceZ = bodyList[1].Position.Z+defaultLoadDistanceZ;
		}
		tempZ.Z = bodyList[8].Position.Z;
		if(player.Position.DistanceTo(tempZ) <= 8)
		{
			bodyList[0].Position += new Vector3(0, 0, spacing.Z);
			bodyList[1].Position += new Vector3(0, 0, spacing.Z);
			bodyList[2].Position += new Vector3(0, 0, spacing.Z);
			bodyList[3].Position += new Vector3(0, 0, spacing.Z);
			bodyList[4].Position += new Vector3(0, 0, spacing.Z);
			bodyList[5].Position += new Vector3(0, 0, spacing.Z);
			bodyList[6].Position += new Vector3(0, 0, spacing.Z);
			bodyList[7].Position += new Vector3(0, 0, spacing.Z);
			bodyList[8].Position += new Vector3(0, 0, spacing.Z);
		}
	}

	bool isPressed = false;	

    private void OnInput(Node camera, InputEvent @event, Vector3 position, Vector3 normal, long shapeIdx)
    {
		player.UpdateAimingArrow(position);
		
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
		{
			// Send moveto signal to player
			// GD.Print($"Clicked plane at position: {position}");
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
