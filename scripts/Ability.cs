using System;
using Godot;

[GlobalClass]
public partial class Ability : Resource
{
	public enum AbilityType
	{
		Melee,
		Ranged
	}
	[Export] public string name {get;set;} = "Slash" ;
	[Export] public int id = -1;
	[Export] public AbilityType type = AbilityType.Melee;
	[Export] public float range = 1.5f;
	[Export] public float cooldown = 1f;
	[Export] public Texture2D img;
}