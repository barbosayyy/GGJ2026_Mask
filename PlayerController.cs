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
				GD.Print(equippedAbilities.ElementAt(abilityID).name);
			break;
			case(1):
				GD.Print("Used abiity 2");
				GD.Print(equippedAbilities.ElementAt(abilityID).name);
			break;
			case(2):
				GD.Print("Used abiity 3");
				GD.Print(equippedAbilities.ElementAt(abilityID).name);
			break;
		}
	}

	public void Possess(Vector3 pPosition)
	{
		Position = pPosition;
		currentPos = Position;
		moveToPos = Position;
	}
}
