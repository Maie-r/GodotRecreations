using Godot;
using System;
using System.Linq;

public partial class UI : CanvasLayer
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		UI CLASS

		Purpose: Display and Update UI elements on the screen

		What was Learned:
			Control nodes are pretty much "Web page builders"
			Displaying buffs durations and stacks ALA risk of rain 2 is actually pretty easy
			Making a Game Over screen is also very simple 

	*/////////////////////////////////////////////////////////////////////////////////

	int PB = 0;
	int Score = 0;
	
	public Player PlayerRef;
	Texture2D LifeImg = (Texture2D)ResourceLoader.Load("res://BulletHell/assets/demoman.jpg");
	Label ScoreLabel;

	HBoxContainer buffcontainer;

	[Signal]
	public delegate void RestartEventHandler();

	bool GameOver = false;

	public override void _Ready()
	{
		ScoreLabel = GetNode<Label>("Score");
		buffcontainer = GetNode<HBoxContainer>("Margin1/Buffs");
	}

	public override void _Process(double delta)
	{
		if (GameOver && Input.IsActionJustPressed("Shoot"))
		{
			EmitSignal(SignalName.Restart);
			Reset();
		}
		UpdateBuffs();
	}

	public void SetLives(int lives)
	{
		var lifecontainer = GetNode<HBoxContainer>("Lives");
		foreach (var child in lifecontainer.GetChildren())
			child.QueueFree();

		for (int i = 0; i < lives; i++)
		{
			var temp = new TextureRect();
			temp.Texture = LifeImg;
			lifecontainer.AddChild(temp);
		}
	}

	public void AddLives(int amount)
	{
		var lifecontainer = GetNode<HBoxContainer>("Lives");
		var count = 0;
		foreach (var child in lifecontainer.GetChildren())
		{
			child.QueueFree();
			count++;
		}
			
		for (int i = 0; i < count+amount; i++)
		{
			var temp = new TextureRect();
			temp.Texture = LifeImg;
			lifecontainer.AddChild(temp);
		}
	}
	
	public void AddScore(int Score)
	{
		this.Score += Score;
		ScoreLabel.Text = this.Score.ToString();
	}

	void UpdateBuffs()
	{
		foreach (var child in buffcontainer.GetChildren())
			child.QueueFree();
		foreach (var kv in PlayerRef.Buffs)
		{
			var tempcount = kv.Value.Count;
			if (tempcount > 0)
			{
				buffcontainer.AddChild(GetBuffIcon(kv.Value.First(), tempcount));
			}
		}
	}

	public Control GetBuffIcon(Buff Buff, int count) // preferrably this should be something the Buff already has, but I didn't want to bloat it
	{
		var ctrl = new Control();
		var timelabel = new Label();
		var iconlabel = new Label();
		var countLabel = new Label();
		timelabel.Position = new Vector2(0, -30);
		ctrl.AddChild(iconlabel); ctrl.AddChild(countLabel); ctrl.AddChild(timelabel);
		countLabel.Scale = new Vector2(0.7f, 0.7f);
		if (count > 1) countLabel.Text = count.ToString();
		timelabel.Text = Buff.DurationLeft.ToString("N2");
		ApplyLabelTheme(iconlabel); ApplyLabelTheme(countLabel); ApplyLabelTheme(timelabel);
		switch (Buff.Category)
		{
			case Buff.BuffCategory.Damage:
				iconlabel.Text = "DMG";
				countLabel.Position = new Vector2(40, 10);
				break;
			case Buff.BuffCategory.FireRate:
				iconlabel.Text = "FR";
				countLabel.Position = new Vector2(20, 10);
				break;
			case Buff.BuffCategory.Health:
				iconlabel.Text = "HP";
				countLabel.Position = new Vector2(20, 10);
				break;
			case Buff.BuffCategory.Speed:
				iconlabel.Text = "SPD";
				countLabel.Position = new Vector2(35, 10);
				break;
			case Buff.BuffCategory.Crit:
				iconlabel.Text = "CRIT";
				countLabel.Position = new Vector2(35, 10);
				break;
			case Buff.BuffCategory.Uber:
				iconlabel.Text = "Uber";
				countLabel.Position = new Vector2(35, 10);
				break;
		}
		return ctrl;
	}

	public void ApplyLabelTheme(Label l)
	{
		l.AddThemeColorOverride("font_shadow_color", new Color(0, 0, 0));
		l.AddThemeColorOverride("font_outline_color", new Color(0, 0, 0));
		l.AddThemeConstantOverride("shadow_outline_size", 5);
	}

	public void Finish()
	{
		if (Score > PB)
			PB = Score;
		GetNode<Label>("GameOver/HBoxContainer/PB").Text = $"Personal Best: {PB}";
		GetNode<Control>("GameOver").Visible = true;
		GameOver = true;
	}

	public void Reset()
	{
		Score = 0;
		SetLives(5);
		GameOver = false;
		AddScore(0);
		GetNode<Control>("GameOver").Visible = false;
	}
}
