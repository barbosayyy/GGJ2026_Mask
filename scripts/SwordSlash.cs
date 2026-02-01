using Godot;
using System.Collections.Generic;

public partial class SwordSlash : Area3D
{
	[Export] public float Damage = 30f;
	[Export] public float SlashRadius = 3f;
	[Export] public float SlashAngle = 120f; // Degrees of the arc

	private Node3D spawner;
	private Vector3 slashDirection;
	private HashSet<Node3D> hitTargets = new HashSet<Node3D>();

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;

		// Auto-destroy after animation
		var timer = GetTree().CreateTimer(0.3f);
		timer.Timeout += () => QueueFree();

		CreateSlashVisual();
	}

	public void Initialize(Vector3 direction, Node3D spawnerNode)
	{
		slashDirection = direction.Normalized();
		spawner = spawnerNode;

		// Rotate to face the slash direction
		if (slashDirection != Vector3.Zero)
		{
			float angle = Mathf.Atan2(slashDirection.X, slashDirection.Z);
			Rotation = new Vector3(0, angle, 0);
		}
	}

	private void OnBodyEntered(Node3D body)
	{
		if (body == spawner) return;
		if (hitTargets.Contains(body)) return;

		// Check if the body is within the slash arc
		Vector3 toTarget = (body.GlobalPosition - GlobalPosition).Normalized();
		toTarget.Y = 0;

		float dot = slashDirection.Dot(toTarget);
		float angleToTarget = Mathf.RadToDeg(Mathf.Acos(Mathf.Clamp(dot, -1f, 1f)));

		if (angleToTarget <= SlashAngle / 2f)
		{
			if (body.HasMethod("TakeDamage"))
			{
				hitTargets.Add(body);
				body.Call("TakeDamage", Damage);
				GD.Print($"Sword slash hit {body.Name} for {Damage} damage!");
			}
		}
	}

	private void CreateSlashVisual()
	{
		var mesh = new MeshInstance3D();
		var torusMesh = new TorusMesh();
		torusMesh.InnerRadius = 0.5f;
		torusMesh.OuterRadius = SlashRadius;
		torusMesh.Rings = 16;
		torusMesh.RingSegments = 8;
		mesh.Mesh = torusMesh;
		mesh.RotationDegrees = new Vector3(90, 0, 0);
		mesh.Position = new Vector3(0, 0.5f, 0);

		var material = new StandardMaterial3D();
		material.AlbedoColor = new Color(0.8f, 0.8f, 1f, 0.7f);
		material.EmissionEnabled = true;
		material.Emission = new Color(0.6f, 0.6f, 1f);
		material.EmissionEnergyMultiplier = 2f;
		material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
		mesh.MaterialOverride = material;

		AddChild(mesh);

		// Animate the slash
		var tween = CreateTween();
		tween.SetParallel(true);
		tween.TweenProperty(mesh, "scale", new Vector3(1.5f, 1.5f, 0.3f), 0.25f)
			.From(new Vector3(0.3f, 0.3f, 1f))
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Expo);
		tween.TweenProperty(material, "albedo_color:a", 0f, 0.25f)
			.SetDelay(0.05f);
	}
}
