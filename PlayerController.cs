using Godot;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

public partial class PlayerController : Node3D
{
	Camera3D cam;
	Healthbar healthbar;

	Vector3 currentPos;
	public Vector3 moveToPos;

	[Export] private float maxHealth = 100;
	private float currentHealth;
	[Export] float velocity = 3.0f;
	[Export] public Godot.Collections.Array<Ability> abilities {get;set;} = new();

	private List<Ability> equippedAbilities = new List<Ability>();

	// debug
	[Export] public bool hasZoom = true;

	// Aiming arrow VFX
	[Export] public Node3D aimingArrow;
	private bool isAimingArrowVisible = false;

	Label scoreLabel;
	public int score;
	// Ability scenes
	[Export] public PackedScene SwordSlashScene;
	[Export] public PackedScene KunaiScene;

	// Ability settings
	[Export] public int KunaiCount = 8;
	[Export] public float AbilityCooldown = 0.5f;
	private float abilityCooldownTimer = 0f;
	protected AnimationPlayer animPlayer;

	// Called when the node enters the scene tree for the first time.

	Node3D skelly;
	Node3D goblin;

	int possessedID;
	public override void _Ready()
	{
		cam = (Camera3D)FindChild("Camera",true,false);
		currentPos = Position;
		cam.MakeCurrent();

		equippedAbilities.Add(abilities.ElementAt(0));
		equippedAbilities.Add(abilities.ElementAt(1));
		equippedAbilities.Add(abilities.ElementAt(2));

		scoreLabel = (Label)GetParent().FindChild("GameUI").FindChild("Score", true, false);

		var healthbars = GetTree().GetNodesInGroup("player_healthbar");
		if (healthbars.Count > 0)
		{
			healthbar = healthbars[0] as Healthbar;
			healthbar.InitHealth(maxHealth);
		}
		
		SetPlayerHealth(maxHealth);


		skelly = (Node3D)FindChild("SkeletonMode", true);
		animPlayer = (AnimationPlayer)skelly.FindChild("AnimationPlayer");
		goblin = (Node3D)FindChild("GoblinMode", true);
		goblin.Hide();
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
			if(animPlayer.IsPlaying() == false)
			{
				animPlayer.Play("WALK",-1, 3, false);
			}
			currentPos += new Vector3(Position.DirectionTo(moveToPos).X * velocity * (float)delta, 0, Position.DirectionTo(moveToPos).Z * velocity * (float)delta);
			Position = currentPos;
			skelly.LookAt(new Vector3(moveToPos.X, 1.589f, moveToPos.Z));
			goblin.LookAt(new Vector3(moveToPos.X, 1.589f, moveToPos.Z));
		}
		else
		{
			if(animPlayer.IsPlaying() == true)
			{
				animPlayer.Stop();
			}
		}

		// Update ability cooldown
		if (abilityCooldownTimer > 0)
		{
			abilityCooldownTimer -= (float)delta;
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
		if (abilityCooldownTimer > 0) return;

		switch(abilityID)
		{
			case(0): // Q - Sword Slash
				GD.Print("Used Sword Slash!");
				SpawnSwordSlash();
				animPlayer.Play("SWORD", -1, 1, false);
				abilityCooldownTimer = AbilityCooldown;
				break;
			case(1): // W - Circular Kunai Throw
				GD.Print("Used Circular Kunai!");
				SpawnCircularKunai();
				abilityCooldownTimer = AbilityCooldown;
				break;
			case(2):
				GD.Print("Used ability 3");
				ShowAimingArrow();
				GD.Print(equippedAbilities.ElementAt(abilityID).name);
				break;
		}
	}

	private Vector3 GetMouseWorldPosition()
	{
		var mousePos = GetViewport().GetMousePosition();
		var from = cam.ProjectRayOrigin(mousePos);
		var to = from + cam.ProjectRayNormal(mousePos) * 1000f;

		var spaceState = GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(from, to);
		query.CollideWithAreas = false;
		query.CollideWithBodies = true;

		var result = spaceState.IntersectRay(query);
		if (result.Count > 0)
		{
			return (Vector3)result["position"];
		}

		// Fallback: intersect with ground plane (Y=0)
		float t = -from.Y / (to - from).Normalized().Y;
		return from + (to - from).Normalized() * t;
	}

	private void SpawnSwordSlash()
	{
		if (SwordSlashScene == null)
		{
			GD.PrintErr("SwordSlashScene not assigned!");
			return;
		}

		var mousePos = GetMouseWorldPosition();
		var direction = (mousePos - GlobalPosition).Normalized();
		direction.Y = 0;

		var slash = SwordSlashScene.Instantiate<SwordSlash>();
		GetTree().Root.AddChild(slash);
		slash.GlobalPosition = GlobalPosition + new Vector3(0, 0.5f, 0);
		slash.Initialize(direction, this);
	}

	private void SpawnCircularKunai()
	{
		if (KunaiScene == null)
		{
			GD.PrintErr("KunaiScene not assigned!");
			return;
		}

		float angleStep = 360f / KunaiCount;

		for (int i = 0; i < KunaiCount; i++)
		{
			float angle = Mathf.DegToRad(i * angleStep);
			var direction = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

			var kunai = KunaiScene.Instantiate<Kunai>();
			GetTree().Root.AddChild(kunai);
			kunai.GlobalPosition = GlobalPosition + new Vector3(0, 1f, 0);
			kunai.Initialize(direction, this);
		}

		var audio = new AudioStreamPlayer3D();
		audio.Stream = GD.Load<AudioStream>("res://Audio/kunai_throw.wav");

		GetTree().Root.AddChild(audio);
		audio.Play();
		audio.Finished += () => audio.QueueFree();
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

	private void SetPlayerHealth(float newHealth)
	{
		currentHealth = newHealth;
		healthbar?.SetHealth(newHealth);
	}

	public virtual void TakeDamage(float damage)
	{
		currentHealth -= damage;
		OnDamageTaken(damage);

		if (currentHealth <= 0)
		{
			Die();
		}
	}

	protected virtual void Die()
	{
		OnDeath();
		QueueFree();
	}

	protected virtual void OnDeath()
	{
		GD.Print("player died!");
	}

	protected virtual void OnDamageTaken(float damage)
	{
		healthbar?.DecreaseHealth(damage);
	}

	public bool IsAimingArrowVisible()
	{
		return isAimingArrowVisible;
	}

	public void Possess(Vector3 pPosition, int enemyID)
	{
		Position = pPosition;
		currentPos = Position;
		moveToPos = Position;
		animPlayer.Stop();

		if(enemyID == 0 || enemyID == 1)
		{
			skelly.Show();
			goblin.Hide();
			animPlayer = (AnimationPlayer)skelly.FindChild("AnimationPlayer");
			Position = new Vector3(pPosition.X, 0f, pPosition.Z);
		}
		else if(enemyID == 2)
		{
			goblin.Show();
			skelly.Hide();
			animPlayer = (AnimationPlayer)goblin.FindChild("AnimationPlayer");
			Position = new Vector3(pPosition.X, 0f, pPosition.Z);
		}
		LookAt(new Vector3(1, 1.4f, 0));
	}

	public void AddScore(int amount)
	{
		score += amount;
		scoreLabel.Text = score.ToString();
	}
}
