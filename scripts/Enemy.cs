using System;
using Godot;

public partial class Enemy : CharacterBody3D
{
	[Export] public EnemyData Data;
	[Export] public Healthbar healthbar;

	protected float currentHealth;
	protected Node3D target;
	protected float attackTimer = 0f;
	protected MeshInstance3D visual;
	protected AnimationPlayer player;
	
	[Signal]
	public delegate void HealthChangedEventHandler(float current, float max);
	

	public override void _Ready()
	{
		if (Data == null)
		{
			QueueFree();
			return;
		}

		currentHealth = Data.MaxHealth;
		SetupVisual();
		var bars = GetTree().GetNodesInGroup("player_healthbar");
		healthbar = bars[0] as Healthbar;


		OnReady();
	}

	protected virtual void OnReady()
	{
		Area3D area = (Area3D)FindChild("Area3D");
		area.InputEvent += OnInput;
		player = (AnimationPlayer)FindChild("AnimationPlayer");
	}

	protected virtual void SetupVisual()
	{
		visual = GetNodeOrNull<MeshInstance3D>("Visual");
		if (visual == null)
		{
			visual = new MeshInstance3D();
			visual.Name = "Visual";
			AddChild(visual);

			var mesh = new BoxMesh();
			mesh.Size = Data.Scale;
			visual.Mesh = mesh;
		}
		var material = new StandardMaterial3D();
		material.AlbedoColor = Data.EnemyColor;
		visual.SetSurfaceOverrideMaterial(0, material);
	}

	private void OnInput(Node camera, InputEvent @event, Vector3 position, Vector3 normal, long shapeIdx)
    {
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
		{
			Camera3D camera3D = GetViewport().GetCamera3D();
			PlayerController player = (PlayerController)camera3D.FindParent("Player");
			player.Possess(Position);
			Die();
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		if (Data == null) return;

		FindTarget();

		if (target != null)
		{
			MoveTowardsTarget((float)delta);
			HandleAttack((float)delta);
		}

		OnPhysicsProcess(delta);
		if(Position.DistanceTo(target.Position) > 2.2f)
		{
			LookAt(target.Position);
		}
		MoveAndSlide();
	}

	protected virtual void OnPhysicsProcess(double delta)
	{
	}

	protected virtual void FindTarget()
	{
		if (target != null) return;

		var players = GetTree().GetNodesInGroup("player");
		if (players.Count > 0)
		{
			target = players[0] as Node3D;
		}
	}

	protected virtual void MoveTowardsTarget(float delta)
	{
		if (target == null) return;

		var direction = (target.GlobalPosition - GlobalPosition).Normalized();

		if (!Data.CanFly)
		{
			direction.Y = 0;
		}
		if(Position.DistanceTo(target.Position) > Data.AttackRange)
		{
			if(Velocity == Vector3.Zero)
			{
				player.Play("WALK", -1, new RandomNumberGenerator().RandfRange(5,7));
			}
			Velocity = direction * Data.MoveSpeed;
		}
		else
		{
			Velocity = Vector3.Zero;
			if(player.CurrentAnimation == "WALK")
			{
				player.Stop();
			}
		}
	}

	protected virtual void HandleAttack(float delta)
	{
		if (target == null) return;

		attackTimer -= delta;

		float distance = GlobalPosition.DistanceTo(target.GlobalPosition);
		if (distance <= Data.AttackRange && attackTimer <= 0)
		{
			Attack();
			attackTimer = Data.AttackCooldown;
		}
	}

	protected virtual void Attack()
	{
		healthbar.DecreaseHealth((float)Data.Damage);
		GD.Print($"{Data.EnemyName} attacks for {Data.Damage} damage!");
		if(Data.EnemyName=="Ranged")
		{
			player.Play("CAST", -1, 2, false);
		}
		else
		{
			player.Play("SWORD", -1, 2, false);
		}
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

	protected virtual void OnDamageTaken(float damage)
	{
	}

	protected virtual void Die()
	{
		OnDeath();
		QueueFree();
	}

	protected virtual void OnDeath()
	{
		GD.Print($"{Data.EnemyName} died!");
	}
}
