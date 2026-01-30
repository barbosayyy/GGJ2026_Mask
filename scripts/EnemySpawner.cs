using Godot;
using System.Collections.Generic;

public partial class EnemySpawner : Node3D
{
	[ExportGroup("Spawn Settings")]
	[Export] public PackedScene[] EnemyScenes = System.Array.Empty<PackedScene>();
	[Export] public float SpawnInterval = 2f;
	[Export] public int EnemiesPerWave = 5;
	[Export] public float SpawnDistance = 15f; // Distance from player to spawn
	[Export] public float SpawnHeight = 0.5f; // Y position for spawned enemies

	[ExportGroup("Spawn Area")]
	[Export] public bool UseCircularSpawn = true; // Spawn in circle around player
	[Export] public Vector2 SpawnAreaSize = new Vector2(20, 20); // for rectangular spawn

	[ExportGroup("Wave Progression")]
	[Export] public bool EnableProgression = true;
	[Export] public float WaveIntervalDecrease = 0.1f; // Decrease spawn time each wave
	[Export] public float MinSpawnInterval = 0.5f;
	[Export] public int EnemyIncreasePerWave = 1; // Add this many enemies each wave

	[ExportGroup("Limits")]
	[Export] public int MaxEnemiesAlive = 100;

	private Node3D player;
	private float spawnTimer = 0f;
	private int currentWave = 0;
	private float currentSpawnInterval;
	private int currentEnemiesPerWave;
	private List<Enemy> aliveEnemies = new List<Enemy>();

	public override void _Ready()
	{
		currentSpawnInterval = SpawnInterval;
		currentEnemiesPerWave = EnemiesPerWave;

		if (EnemyScenes.Length == 0)
		{
			GD.PrintErr("EnemySpawner: No enemy scenes assigned!");
		}
	}

	public override void _Process(double delta)
	{
		FindPlayer();

		if (player == null || EnemyScenes.Length == 0)
			return;

		// Clean up dead enemies from list
		aliveEnemies.RemoveAll(e => !IsInstanceValid(e));

		spawnTimer -= (float)delta;

		if (spawnTimer <= 0)
		{
			SpawnWave();
			spawnTimer = currentSpawnInterval;
		}
	}

	private void FindPlayer()
	{
		if (player != null) return;

		var players = GetTree().GetNodesInGroup("player");
		if (players.Count > 0)
		{
			player = players[0] as Node3D;
		}
	}

	private void SpawnWave()
	{
		if (aliveEnemies.Count >= MaxEnemiesAlive)
		{
			GD.Print("Max enemies reached, skipping spawn");
			return;
		}

		currentWave++;

		int enemiesToSpawn = Mathf.Min(currentEnemiesPerWave, MaxEnemiesAlive - aliveEnemies.Count);

		for (int i = 0; i < enemiesToSpawn; i++)
		{
			SpawnEnemy();
		}

		if (EnableProgression)
		{
			ProgressDifficulty();
		}

		GD.Print($"Wave {currentWave}: Spawned {enemiesToSpawn} enemies. Total alive: {aliveEnemies.Count}");
	}

	private void SpawnEnemy()
	{
		if (player == null || EnemyScenes.Length == 0)
			return;

		// Pick random enemy type
		var randomScene = EnemyScenes[GD.RandRange(0, EnemyScenes.Length - 1)];
		var enemy = randomScene.Instantiate<Enemy>();

		if (enemy == null)
		{
			GD.PrintErr("Failed to instantiate enemy!");
			return;
		}

		// Calculate spawn position
		Vector3 spawnPos = GetSpawnPosition();
		enemy.GlobalPosition = spawnPos;

		// Add to scene
		GetTree().Root.AddChild(enemy);
		aliveEnemies.Add(enemy);
	}

	private Vector3 GetSpawnPosition()
	{
		Vector3 playerPos = player.GlobalPosition;

		if (UseCircularSpawn)
		{
			// Spawn in a circle around the player
			float angle = GD.Randf() * Mathf.Tau; // Random angle
			float distance = SpawnDistance + GD.Randf() * 2f - 1f; // Slight variation

			return new Vector3(
				playerPos.X + Mathf.Cos(angle) * distance,
				SpawnHeight,
				playerPos.Z + Mathf.Sin(angle) * distance
			);
		}
		else
		{
			// Spawn in rectangular area around player
			return new Vector3(
				playerPos.X + GD.Randf() * SpawnAreaSize.X - SpawnAreaSize.X / 2f,
				SpawnHeight,
				playerPos.Z + GD.Randf() * SpawnAreaSize.Y - SpawnAreaSize.Y / 2f
			);
		}
	}

	private void ProgressDifficulty()
	{
		// Decrease spawn interval (spawn more frequently)
		currentSpawnInterval = Mathf.Max(
			MinSpawnInterval,
			currentSpawnInterval - WaveIntervalDecrease
		);

		// Increase enemies per wave
		currentEnemiesPerWave += EnemyIncreasePerWave;
	}

	public void ForceSpawnWave()
	{
		SpawnWave();
	}

	public void ResetProgression()
	{
		currentWave = 0;
		currentSpawnInterval = SpawnInterval;
		currentEnemiesPerWave = EnemiesPerWave;
	}
}
