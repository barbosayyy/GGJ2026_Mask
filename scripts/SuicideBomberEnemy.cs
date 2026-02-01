using Godot;

public partial class SuicideBomberEnemy : Enemy
{
	[Export] public float ExplosionRadius = 3f;
	[Export] public float ExplosionDamage = 30f;
	[Export] public float FuseTime = 1f;

	private float fuseTimer = -1f;
	private bool isFused = false;
	private float pulseTimer = 0f;
	private StandardMaterial3D visualMaterial;

	protected override void OnReady()
	{
		base.OnReady();

		if (visual != null)
		{
			visualMaterial = visual.GetSurfaceOverrideMaterial(0) as StandardMaterial3D;
		}
	}

	protected override void OnPhysicsProcess(double delta)
	{
		base.OnPhysicsProcess(delta);

		if (isFused)
		{
			fuseTimer -= (float)delta;

			pulseTimer += (float)delta * 10f;
			if (visualMaterial != null)
			{
				float pulse = Mathf.Abs(Mathf.Sin(pulseTimer));
				visualMaterial.EmissionEnabled = true;
				visualMaterial.Emission = Data.EnemyColor * pulse * 2f;
			}

			if (visual != null)
			{
				float scaleMultiplier = 1f + (1f - (fuseTimer / FuseTime)) * 0.3f;
				visual.Scale = Vector3.One * scaleMultiplier;
			}

			if (fuseTimer <= 0)
			{
				Explode();
			}
		}
	}

	protected override void HandleAttack(float delta)
	{
		if (target == null) return;

		float distance = GlobalPosition.DistanceTo(target.GlobalPosition);

		if (distance <= Data.AttackRange && !isFused)
		{
			isFused = true;
			fuseTimer = FuseTime;
			GD.Print($"{Data.EnemyName} is about to explode!");
		}
	}

	protected override void Attack()
	{

	}

	private void Explode()
	{
		GD.Print($"{Data.EnemyName} explodes!");

		var spaceState = GetWorld3D().DirectSpaceState;

		if (target != null && GlobalPosition.DistanceTo(target.GlobalPosition) <= ExplosionRadius)
		{
			if (target.HasMethod("TakeDamage"))
			{
				target.Call("TakeDamage", ExplosionDamage);
			}
		}

		// var audio = new AudioStreamPlayer3D();
		// audio.Stream = GD.Load<AudioStream>("res://Audio/zombie_hit.wav");
		// AddChild(audio);
		// audio.Play();
		// audio.Finished += () => audio.QueueFree();

		// TODO: particle effects, screen shake (maybe?)
		Die();
	}

	protected override void OnDeath()
	{
		base.OnDeath();
	}

	public override void TakeDamage(float damage)
	{
		base.TakeDamage(damage);

		if (isFused)
		{
			fuseTimer = Mathf.Max(0.1f, fuseTimer - 0.2f);
		}
	}
}
