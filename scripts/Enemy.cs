using Godot;

public partial class Enemy : CharacterBody3D
{
	[Export] public EnemyData Data;
	[Export] public Healthbar healthbar;

	protected float currentHealth;
	protected Node3D target;
	protected float attackTimer = 0f;
	protected MeshInstance3D visual;
	
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


		GD.Print($"HEREHEREHERE {healthbar} aaaaa");
		GD.Print($"aaaaaa");
		OnReady();
	}

	protected virtual void OnReady()
	{
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

		Velocity = direction * Data.MoveSpeed;
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
