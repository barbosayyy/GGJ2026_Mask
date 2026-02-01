using Godot;


public partial class RangedEnemy : Enemy
{
	[Export] public float PreferredDistance = 8f; 
	[Export] public float MinDistance = 4f; 
	[Export] public PackedScene ProjectileScene;
	[Export] public float ProjectileSpeed = 10f;

	protected override void MoveTowardsTarget(float delta)
	{
		if (target == null) return;

		float distance = GlobalPosition.DistanceTo(target.GlobalPosition);
		var direction = (target.GlobalPosition - GlobalPosition).Normalized();
		direction.Y = 0; 

		
		if (distance < MinDistance)
		{
			
			Velocity = -direction * Data.MoveSpeed;
		}
		else if (distance > PreferredDistance)
		{
			
			Velocity = direction * Data.MoveSpeed * 0.5f; 
		}
		else
		{
			
			var strafeDirection = new Vector3(-direction.Z, 0, direction.X); 
			Velocity = strafeDirection * Data.MoveSpeed * 0.3f;
		}
	}

	protected override void Attack()
	{
		if (target == null) return;

		ShootProjectile();
	}

	private void ShootProjectile()
	{
		if (ProjectileScene == null)
		{
			CreateDefaultProjectile();
		}
		else
		{
			var projectile = ProjectileScene.Instantiate<Node3D>();

			var spawnPos = GlobalPosition + Vector3.Up * 1.5f;
			var direction = (target.GlobalPosition - GlobalPosition).Normalized();

			GetParent().AddChild(projectile);
			projectile.GlobalPosition = spawnPos;

			if (projectile.HasMethod("Initialize"))
			{
				projectile.Call("Initialize", direction, ProjectileSpeed, Data.Damage, this);
			}
		}

		GD.Print($"{Data.EnemyName} shoots a projectile!");
	}

	private void CreateDefaultProjectile()
	{
		
		var projectile = new Area3D();
		GetParent().AddChild(projectile);
		projectile.GlobalPosition = GlobalPosition + Vector3.Up * 1.5f;

		
		var mesh = new MeshInstance3D();
		var sphereMesh = new SphereMesh();
		sphereMesh.Radius = 0.2f;
		mesh.Mesh = sphereMesh;

		var material = new StandardMaterial3D();
		material.AlbedoColor = Data.EnemyColor;
		material.EmissionEnabled = true;
		material.Emission = Data.EnemyColor;
		mesh.SetSurfaceOverrideMaterial(0, material);
		projectile.AddChild(mesh);

		
		var collision = new CollisionShape3D();
		var shape = new SphereShape3D();
		shape.Radius = 0.2f;
		collision.Shape = shape;
		projectile.AddChild(collision);

		
		var direction = (target.GlobalPosition - GlobalPosition).Normalized();
		var script = GD.Load<Script>("res://Projectile.cs");
		if (script != null)
		{
			projectile.SetScript(script);
			if (projectile.HasMethod("Initialize"))
			{
				projectile.Call("Initialize", direction, ProjectileSpeed, Data.Damage, this);
			}
		}
	}

	protected override void OnDamageTaken(float damage)
	{
		base.OnDamageTaken(damage);

		if (target != null && GD.Randf() < 0.3f) 
		{
			TeleportAway();
		}
	}

	private void TeleportAway()
	{
		if (target == null) return;

		var directionAway = (GlobalPosition - target.GlobalPosition).Normalized();
		var randomOffset = new Vector3(
			(float)(GD.Randf() - 0.5f) * 4f,
			0,
			(float)(GD.Randf() - 0.5f) * 4f
		);

		var newPosition = GlobalPosition + directionAway * 3f + randomOffset;
		GlobalPosition = newPosition;

		GD.Print($"{Data.EnemyName} teleports away!");
	}
}
