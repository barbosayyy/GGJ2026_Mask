using Godot;

public partial class Healthbar : ProgressBar
{
	private Timer timer;
	private ProgressBar damageBar;
	private TextureProgressBar maskBar;

	private float hostHealth;
	private float maskHealth;

	public override void _Ready()
	{
		AddToGroup("player_healthbar");
		timer = GetNode<Timer>("Timer");
		damageBar = GetNode<ProgressBar>("DamageBar");
		maskBar = GetNode<TextureProgressBar>("TextureProgressBar");;

		timer.Timeout += OnTimerTimeout;
		InitHealth(100);
	}
	
	public void SetMaskHealth(float newHealth) 
	{
		maskHealth = (float) Mathf.Min(maskBar.MaxValue, newHealth);
		maskBar.Value = maskHealth;
	}

	public void SetHealth(float newHealth)
	{
		float prevHealth = hostHealth;
		hostHealth = (float) Mathf.Min(MaxValue, newHealth);

		if (hostHealth < 0)
		{
			//QueueFree();
			return;
		}

		Value = hostHealth;
		
		if (hostHealth < prevHealth)
		{
			timer.Start();
		}
		else
		{
			damageBar.Value = hostHealth;
		}
	}
	
	public void DecreaseHealth(float damage) 
	{
		SetHealth(hostHealth-damage);
		SetMaskHealth(maskHealth-(float)(damage*0.1));
	}

	public void InitHealth(float initialHealth)
	{
		hostHealth = initialHealth;
		maskHealth = initialHealth;

		MaxValue = initialHealth;
		Value = initialHealth;

		damageBar.MaxValue = initialHealth;
		damageBar.Value = initialHealth;
		maskBar.MaxValue = initialHealth;
		maskBar.Value = initialHealth;
	}

	private void OnTimerTimeout()
	{
		damageBar.Value = hostHealth;
	}
}
