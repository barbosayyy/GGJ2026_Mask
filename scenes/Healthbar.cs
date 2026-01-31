using Godot;

public partial class Healthbar : ProgressBar
{
	private Timer timer;
	private ProgressBar damageBar;

	private float health;

	public override void _Ready()
	{
		AddToGroup("player_healthbar");
		timer = GetNode<Timer>("Timer");
		damageBar = GetNode<ProgressBar>("DamageBar");

		timer.Timeout += OnTimerTimeout;
		InitHealth(100);
	}

	public void SetHealth(float newHealth)
	{
		float prevHealth = health;
		health = (float) Mathf.Min(MaxValue, newHealth);

		if (health < 0)
		{
			QueueFree();
			return;
		}

		Value = health;

		if (health < prevHealth)
		{
			timer.Start();
		}
		else
		{
			damageBar.Value = health;
		}
	}
	
	public void DecreaseHealth(float newHealth) 
	{
		SetHealth(health-newHealth);
	}

	public void InitHealth(float initialHealth)
	{
		health = initialHealth;

		MaxValue = initialHealth;
		Value = initialHealth;

		damageBar.MaxValue = initialHealth;
		damageBar.Value = initialHealth;
	}

	private void OnTimerTimeout()
	{
		damageBar.Value = health;
	}
}
