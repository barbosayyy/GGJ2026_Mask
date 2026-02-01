using Godot;

public partial class Kunai : Area3D
{
	[Export] public float Speed = 15f;
	[Export] public float Damage = 20f;

	private Vector3 direction;
	private Node3D spawner;
	private float lifetime = 5f;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;
	}

	public void Initialize(Vector3 dir, Node3D spawnerNode)
	{
		direction = dir.Normalized();
		spawner = spawnerNode;

		if (direction != Vector3.Zero)
		{
			LookAt(GlobalPosition + direction, Vector3.Up);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition += direction * Speed * (float)delta;

		lifetime -= (float)delta;
		if (lifetime <= 0)
		{
			QueueFree();
		}
	}

	private void OnBodyEntered(Node3D body)
	{
		if (body == spawner) return;

		if (body.HasMethod("TakeDamage"))
		{
			body.Call("TakeDamage", Damage);
			GD.Print($"Kunai hit {body.Name} for {Damage} damage!");
		}
		QueueFree();
	}

	private void OnAreaEntered(Area3D area)
	{
		if (area.GetParent() == spawner) return;
		// Don't destroy on other kunais
		if (area is Kunai) return;
	}
}
