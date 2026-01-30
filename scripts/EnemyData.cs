using Godot;

[GlobalClass]
public partial class EnemyData : Resource
{
	[Export] public string EnemyName = "Enemy";
	[Export] public float MaxHealth = 100f;
	[Export] public float MoveSpeed = 3f;
	[Export] public float Damage = 10f;
	[Export] public float AttackRange = 1.5f;
	[Export] public float AttackCooldown = 1f;
	[Export] public int ExperienceValue = 5;

	[Export] public Color EnemyColor = Colors.Red;
	[Export] public Vector3 Scale = Vector3.One;

	[Export] public float KnockbackResistance = 0f; // 0 = full knockback, 1 = immune
	[Export] public bool CanFly = false;
}
