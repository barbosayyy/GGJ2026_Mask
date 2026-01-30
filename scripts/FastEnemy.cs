using Godot;

/// <summary>
/// Example of a custom enemy type that extends the base Enemy class.
/// This enemy has faster movement and different attack behavior.
/// </summary>
public partial class FastEnemy : Enemy
{
	[Export] public float DashSpeed = 10f;
	[Export] public float DashCooldown = 3f;

	private float dashTimer = 0f;
	private bool isDashing = false;
	private float dashDuration = 0.5f;
	private float currentDashTime = 0f;

	protected override void OnPhysicsProcess(double delta)
	{
		base.OnPhysicsProcess(delta);

		if (isDashing)
		{
			currentDashTime -= (float)delta;
			if (currentDashTime <= 0)
			{
				isDashing = false;
			}
		}
		else
		{
			dashTimer -= (float)delta;
		}
	}

	protected override void MoveTowardsTarget(float delta)
	{
		if (target == null) return;

		var direction = (target.GlobalPosition - GlobalPosition).Normalized();
		direction.Y = 0;

		// Dash ability
		if (!isDashing && dashTimer <= 0 && GlobalPosition.DistanceTo(target.GlobalPosition) > Data.AttackRange)
		{
			isDashing = true;
			currentDashTime = dashDuration;
			dashTimer = DashCooldown;
		}

		float speed = isDashing ? DashSpeed : Data.MoveSpeed;
		Velocity = direction * speed;
	}

	protected override void OnDamageTaken(float damage)
	{
		base.OnDamageTaken(damage);
		// Flash effect or play sound here
	}
}
