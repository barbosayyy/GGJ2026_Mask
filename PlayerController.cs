using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerController : Node3D
{
	Camera3D cam;

	Vector3 currentPos;
	public Vector3 moveToPos;
	[Export] float velocity = 3.0f;
	[Export] public Godot.Collections.Array<Ability> abilities {get;set;} = new();

	private List<Ability> equippedAbilities = new List<Ability>();

	// debug
	[Export] public bool hasZoom = true;

	// Aiming arrow VFX
	[Export] public Node3D aimingArrow;
	private bool isAimingArrowVisible = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cam = (Camera3D)FindChild("Camera",true,false);
		currentPos = Position;
		cam.MakeCurrent();

		equippedAbilities.Add(abilities.ElementAt(0));
		equippedAbilities.Add(abilities.ElementAt(1));
		equippedAbilities.Add(abilities.ElementAt(2));
	}

    public override void _EnterTree()
    {
        base._EnterTree();
		HideAimingArrow();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(currentPos.DistanceSquaredTo(moveToPos) > 0.5)
		{
			currentPos += new Vector3(Position.DirectionTo(moveToPos).X * velocity * (float)delta, 0, Position.DirectionTo(moveToPos).Z * velocity * (float)delta);
			Position = currentPos;
		}		
	}

	public override void _Input(InputEvent @event)
	{
		if(hasZoom){
			if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.WheelUp && mouseEvent.Pressed)
			{
				cam.Position -= new Vector3(0, 1, 1);
			}
			else if(@event is InputEventMouseButton mEvent && mEvent.ButtonIndex == MouseButton.WheelDown && mEvent.Pressed)
			{
				cam.Position += new Vector3(0, 1, 1);
			}
		}

		if (@event.IsActionPressed("Ability1"))
		{
			UseAbility(equippedAbilities.ElementAt(0).id);
		}
		if (@event.IsActionPressed("Ability2"))
		{
			UseAbility(equippedAbilities.ElementAt(1).id);
		}
		if (@event.IsActionPressed("Ability3"))
		{
			UseAbility(equippedAbilities.ElementAt(2).id);
		}
	}
	
	// Ability behaviors
	private void UseAbility(int abilityID)
	{
		switch(abilityID)
		{
			case(0):
				GD.Print("Used abiity 1");
				ShowAimingArrow();
				GD.Print(equippedAbilities.ElementAt(abilityID).name);
			break;
			case(1):
				GD.Print("Used abiity 2");
				ShowAimingArrow();
				GD.Print(equippedAbilities.ElementAt(abilityID).name);
			break;
			case(2):
				GD.Print("Used abiity 3");
				ShowAimingArrow();
				GD.Print(equippedAbilities.ElementAt(abilityID).name);
			break;
		}
	}

	// Aiming Arrow Interface
	public void ShowAimingArrow()
	{
		if (aimingArrow == null) return;
		GD.Print("Showing arrow: " + aimingArrow.Name);
		aimingArrow.Show();
		isAimingArrowVisible = true;
	}

	public void HideAimingArrow()
	{
		if (aimingArrow == null)
		{
			GD.Print("aimingArrow is null - assign it in the inspector!");
			return;
		}
		GD.Print("Hiding arrow: " + aimingArrow.Name);
		aimingArrow.Hide();
		isAimingArrowVisible = false;
	}

	public void SetAimingArrowDirection(Vector3 direction)
	{
		if (aimingArrow == null) return;
		// Calculate Y rotation angle from direction (for top-down view)
		float angle = Mathf.Atan2(direction.X, direction.Z);
		aimingArrow.Rotation = new Vector3(aimingArrow.Rotation.X, angle, aimingArrow.Rotation.Z);
	}

	public void UpdateAimingArrow(Vector3 targetPosition)
	{
		if (aimingArrow == null) return;
		Vector3 direction = (targetPosition - Position).Normalized();
		direction.Y = 1; // Keep it flat on the ground plane
		SetAimingArrowDirection(direction);
	}

	public bool IsAimingArrowVisible()
	{
		return isAimingArrowVisible;
	}
	
	public void Possess(Vector3 pPosition)
	{
		Position = pPosition;
		currentPos = Position;
		moveToPos = Position;
	}
}
