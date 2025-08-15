using Godot;
using System;
using System.Collections;

public partial class SlidingLabel : Node2D
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		SLIDING LABLE CLASS

		Purpose: Does what it says on the tin, mostly to replicate TF2's Damage numbers and Critical hit effects

		What was Learned:
			This is pretty easy, although most complexity is changing the text overrides, which is done outside of the class

	*/////////////////////////////////////////////////////////////////////////////////

	[Export]
	public string Text = "";
	[Export]
	public float Opacity = 1;
	[Export]
	public Vector2 SlideDirection = new Vector2(0, -1);
	[Export]
	public float Speed = 100;
	[Export]
	public float LingerTime = 2;
	Timer LingerTimer;
	[Export]
	public bool FadeOut = true;

	public Label labelref;

	bool IsFading;

	float fadeCoef = 0;

	public float MaterialAlpha
	{
		get
		{
			if (this.Material is ShaderMaterial sm)
				return (float)sm.GetShaderParameter("Alpha");
			else throw new Exception("Sliding label doesn't have the correct shader to get");
		}
		set
		{
			if (this.Material is ShaderMaterial sm)
				sm.SetShaderParameter("Alpha", value);
			else throw new Exception("Sliding label doesn't have the correct shader to set");
		}
	}

	public override void _Ready()
	{
		Material = Material.Duplicate() as ShaderMaterial; // so not every label is the same (i've been through this twice)
		labelref = GetLabel();
		//GD.Print(MaterialColor);
		MaterialAlpha = Opacity;
		labelref.Text = Text;
		if (LingerTime > 0) 
		{
			LingerTimer = TempTimer.Get(LingerTime);
			LingerTimer.Timeout += QueueFree;
			AddChild(LingerTimer);
			fadeCoef = -1/LingerTime;
		}
	}

	public override void _Process(double delta)
	{
		this.Position += SlideDirection * Speed * (float)delta;
		if (FadeOut) MaterialAlpha += fadeCoef * (float)delta;
	}

	public Label GetLabel()
	{
		return GetNode<Label>("Label");
	}
}
