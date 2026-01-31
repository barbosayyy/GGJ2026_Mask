using Godot;


// temp player
public partial class Player : CharacterBody3D
{
	[Export] public float Speed = 5f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] public float MaxHealth = 100f;
	[Export] public Healthbar healthbar;

	private float currentHealth;
	private float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready()
	{
		AddToGroup("player");
		currentHealth = MaxHealth;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = new Vector3(inputDir.X, 0, inputDir.Y).Normalized();

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public void TakeDamage(float damage)
	{
		currentHealth -= damage;
		GD.Print($"Player took {damage} damage! Health: {currentHealth}/{MaxHealth}");

		if (currentHealth <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		GD.Print("Player died!");
		//TODO: death
		GetTree().ReloadCurrentScene();
	}
}
