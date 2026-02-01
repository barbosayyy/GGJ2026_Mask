using Godot;


public partial class Projectile : Area3D
{
	[Export] public float ExplosionRadius = 5f;
	[Export] public float ExplosionDamage = 50f;
	[Export] public float ExplosionScale = 3f;

	private Vector3 direction;
	private float speed;
	private float damage;
	private float lifetime = 10f;
	private Node3D spawner;
	private bool hasExploded = false;

	public override void _Ready()
	{
		// Defer signal connection to next frame so Initialize() can set spawner first
		CallDeferred(MethodName.ConnectSignals);
	}

	private void ConnectSignals()
	{
		BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;
	}

	public void Initialize(Vector3 dir, float spd, float dmg, Node3D spawnerNode = null)
	{
		direction = dir.Normalized();
		speed = spd;
		damage = dmg;
		spawner = spawnerNode;

		// Face the direction of travel (toward player)
		if (direction != Vector3.Zero)
		{
			LookAt(GlobalPosition + direction, Vector3.Up);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition += direction * speed * (float)delta;

		lifetime -= (float)delta;
		if (lifetime <= 0)
		{
			QueueFree();
		}
	}

	private void OnBodyEntered(Node3D body)
	{
		if (body == spawner) return;

		// Ignore ground/terrain (StaticBody3D) - only react to characters
		if (body is StaticBody3D) return;

		if (body.IsInGroup("player"))
		{
			if (body.HasMethod("TakeDamage"))
			{
				body.Call("TakeDamage", damage);
			}
			QueueFree();
		}
	}

	private void OnAreaEntered(Area3D area)
	{
		// Only react to other projectiles for explosion mechanic
		if (area is Projectile otherProjectile && !hasExploded && !otherProjectile.hasExploded)
		{
			hasExploded = true;
			otherProjectile.hasExploded = true;

			Vector3 explosionPoint = (GlobalPosition + otherProjectile.GlobalPosition) / 2f;

			FireBallExplosion(explosionPoint);

			otherProjectile.QueueFree();
			QueueFree();
		}

		if (area.IsInGroup("player"))
		{
			var player = area.GetParent();
			if (player != null && player.HasMethod("TakeDamage"))
			{
				player.Call("TakeDamage", damage);
			}
			QueueFree();
		}
	}

	private void FireBallExplosion(Vector3 position)
	{
		var spaceState = GetWorld3D().DirectSpaceState;
		var shape = new SphereShape3D();
		shape.Radius = ExplosionRadius;

		var query = new PhysicsShapeQueryParameters3D();
		query.Shape = shape;
		query.Transform = new Transform3D(Basis.Identity, position);
		query.CollideWithAreas = false;
		query.CollideWithBodies = true;

		var results = spaceState.IntersectShape(query);

		foreach (var result in results)
		{
			if (result.TryGetValue("collider", out var colliderVariant))
			{
				var collider = colliderVariant.As<Node3D>();
				if (collider != null && collider != spawner)
				{
					if (collider.HasMethod("TakeDamage"))
					{
						float distance = position.DistanceTo(collider.GlobalPosition);
						float damageMultiplier = 1f - (distance / ExplosionRadius);
						float finalDamage = ExplosionDamage * Mathf.Max(0.3f, damageMultiplier);

						collider.Call("TakeDamage", finalDamage);
						GD.Print($"Explosion hit {collider.Name} for {finalDamage} damage!");
					}
				}
			}
		}

		CreateExplosionVisual(position);
	}

	private void CreateExplosionVisual(Vector3 position)
	{
		var explosionNode = new Node3D();
		explosionNode.GlobalPosition = position;
		GetTree().Root.AddChild(explosionNode);

		var mesh = new MeshInstance3D();
		var sphereMesh = new SphereMesh();
		sphereMesh.Radius = 0.5f;
		sphereMesh.Height = 1f;
		mesh.Mesh = sphereMesh;

		var material = new StandardMaterial3D();
		material.AlbedoColor = new Color(1f, 0.5f, 0f);
		material.EmissionEnabled = true;
		material.Emission = new Color(1f, 0.3f, 0f);
		material.EmissionEnergyMultiplier = 3f;
		material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
		mesh.MaterialOverride = material;

		explosionNode.AddChild(mesh);

		var tween = explosionNode.CreateTween();
		tween.SetParallel(true);

		tween.TweenProperty(mesh, "scale", Vector3.One * ExplosionScale, 0.3f)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Expo);

		tween.TweenProperty(material, "albedo_color:a", 0f, 0.4f)
			.SetDelay(0.1f);

		tween.SetParallel(false);
		tween.TweenCallback(Callable.From(() => explosionNode.QueueFree()))
			.SetDelay(0.5f);
	}
}
