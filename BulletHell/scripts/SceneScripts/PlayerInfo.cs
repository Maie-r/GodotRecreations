using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerInfo : CanvasLayer
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		PLAYERINFO CLASS

		Purpose: Display a debug menu with F3 that displays the exact status values of the player

		What was Learned:
			This is pretty easy and pretty useful, even more so if I make it interactive

	*/////////////////////////////////////////////////////////////////////////////////

	Player player;
	bool IsOpen = false;
	
	Godot.Collections.Array<Node> labels;

	public override void _Ready()
	{
		labels = GetNode<VBoxContainer>("Labels").GetChildren();
	}

	public override void _Process(double delta)
	{
		if (this.Visible && player != null)
		{
			for (int i = 0; i < labels.Count; i++)
				if (labels[i] is Label l)
				{
					switch (i)
					{
						case 0:
							l.Text = $"Health: {player.Lives}";
							break;
						case 1:
							l.Text = $"Damage: {player.Damage}";
							break;
						case 2:
							l.Text = $"FireRate: {(1/player.CurrentWeapon.FireCoolDown).ToString("N2")}bps ({player.FireCoolDown}x)";
							break;
						case 3:
							l.Text = $"Speed: {player.Speed}";
							break;

					}
				}
		}
	}
	
	public void SetPlayer(Player p)
	{
		player = p;
	}
	
	public void ToggleVis()
	{
		this.Visible = !this.Visible;
	}
}
