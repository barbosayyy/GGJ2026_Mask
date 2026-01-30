using Godot;


public partial class Projectile : Area3D
{
	private Vector3 direction;
	private float speed;
	private float damage;
	private float lifetime = 5f;
	
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;
	}

	public void Initialize(Vector3 dir, float spd, float dmg)
	{
		direction = dir.Normalized();
		speed = spd;
		damage = dmg;
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
		if (body.IsInGroup("player"))
		{
			if (body.HasMethod("TakeDamage"))
			{
				body.Call("TakeDamage", damage);
			}
			QueueFree();
		}
		else if (body is StaticBody3D or CharacterBody3D)
		{
			QueueFree();
		}
	}

	private void OnAreaEntered(Area3D area)
	{
		QueueFree();
	}
}
