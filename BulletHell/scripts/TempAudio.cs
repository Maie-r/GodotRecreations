using System;
using Godot;

/// <summary>
/// For playing audio that self disposes, connected to the reference node
/// </summary>
public static class TempAudio
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		TEMP AUDIO CLASS

		Purpose: Play a sound once and dispose itself when ended

		What was Learned:
			I feel like this would be my prefered way of playing audio most of the time, instead of having
				hundreds of AudioStreamPlayers in the scenes themselves, I gotta look up if theres any advantage to that
			Audio stops playing if the Parent is disposed (Whoops) > Lead into me making the TempAudioGlobal class
			AudioStreamPlayer has a pitch property which is really cool

	*/////////////////////////////////////////////////////////////////////////////////

    static string audiofolder = "res://BulletHell/assets/sfx/";

	/// <summary>
	/// Plays the sound with a specific name from the static audio folder, disposes itself when finished
	/// </summary>
    public static void Play(string name, Node2D reference)
	{
		var t = new AudioStreamPlayer2D();
		t.Stream = (AudioStream)ResourceLoader.Load(audiofolder + name);
		t.Finished += () => t.QueueFree();
		reference.AddChild(t);
		t.Play();
	}

	public static void Play(string name, float volumeDb, Node2D reference)
	{
		var t = new AudioStreamPlayer2D();
		t.VolumeDb = volumeDb;
		t.Stream = (AudioStream)ResourceLoader.Load(audiofolder + name);
		t.Finished += () => t.QueueFree();
		reference.AddChild(t);
		t.Play();
	}

	public static void PlayAndDispose(string name, Node2D reference)
	{
		var t = new AudioStreamPlayer2D();
		t.Stream = (AudioStream)ResourceLoader.Load(audiofolder + name);
		t.Finished += () => reference.QueueFree();
		reference.AddChild(t);
		t.Play();
	}

	public static void PlayAndDispose(string name, float volumeDb, Node2D reference)
	{
		var t = new AudioStreamPlayer2D();
		t.VolumeDb = volumeDb;
		t.Stream = (AudioStream)ResourceLoader.Load(audiofolder + name);
		t.Finished += () => reference.QueueFree();
		reference.AddChild(t);
		t.Play();
	}

    public static void PlayRandomPitch(string name, float pitchvariance, Node2D reference)
	{
		var t = new AudioStreamPlayer2D();
		t.Stream = (AudioStream)ResourceLoader.Load(audiofolder + name);
		t.Finished += () => t.QueueFree();
		reference.AddChild(t);
        RandomNumberGenerator rng = new RandomNumberGenerator();
        var pitch = 1 + rng.RandfRange(-pitchvariance, pitchvariance);
        if (pitch < 0) pitch = 0.1f;
        t.PitchScale = pitch;
		t.Play();
	}

	public static void PlayRandomPitch(string name, float pitchvariance, float volumeDb, Node2D reference)
	{
		var t = new AudioStreamPlayer2D();
		t.VolumeDb = volumeDb;
		t.Stream = (AudioStream)ResourceLoader.Load(audiofolder + name);
		t.Finished += () => t.QueueFree();
		reference.AddChild(t);
        RandomNumberGenerator rng = new RandomNumberGenerator();
        var pitch = 1 + rng.RandfRange(-pitchvariance, pitchvariance);
        if (pitch < 0) pitch = 0.1f;
        t.PitchScale = pitch;
		t.Play();
	}

	public static void PlayRandomPitchAndDispose(string name, float pitchvariance, Node2D reference)
	{
		var t = new AudioStreamPlayer2D();
		t.Stream = (AudioStream)ResourceLoader.Load(audiofolder + name);
		t.Finished += () => reference.QueueFree();
		reference.AddChild(t);
        RandomNumberGenerator rng = new RandomNumberGenerator();
        var pitch = 1 + rng.RandfRange(-pitchvariance, pitchvariance);
        if (pitch < 0) pitch = 0.1f;
        t.PitchScale = pitch;
		t.Play();
	}

	public static void PlayRandomPitchAndDispose(string name, float pitchvariance, float volumeDb, Node2D reference)
	{
		var t = new AudioStreamPlayer2D();
		t.VolumeDb = volumeDb;
		t.Stream = (AudioStream)ResourceLoader.Load(audiofolder + name);
		t.Finished += () => reference.QueueFree();
		reference.AddChild(t);
        RandomNumberGenerator rng = new RandomNumberGenerator();
        var pitch = 1 + rng.RandfRange(-pitchvariance, pitchvariance);
        if (pitch < 0) pitch = 0.1f;
        t.PitchScale = pitch;
		t.Play();
	}
}

/// <summary>
/// For playing audio globally, that self disposes
/// </summary>
public static class TempAudioGlobal
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		TEMP AUDIO GLOBAL CLASS

		Purpose: Does exactly the same things as TempAudio, but the AudioStreamPlayer is Added to the Root of the tree

		What was Learned:
			This does work, but It's prob best practice to actually instantiate whatever is calling this outside of 
				their parent, if that's an issue

	*/////////////////////////////////////////////////////////////////////////////////
	
    static string audiofolder = "res://BulletHell/assets/sfx/";

	/// <summary>
	/// Plays the sound with a specific name from the static audio folder, disposes itself when finished
	/// </summary>
    public static void Play(string name, Node reference)
	{
		var t = new AudioStreamPlayer2D();
		t.Stream = (AudioStream)ResourceLoader.Load(audiofolder + name);
		t.Finished += () => t.QueueFree();
		reference.GetTree().Root.AddChild(t);
		t.Play();
	}

	public static void Play(string name, float volumeDb, Node reference)
	{
		var t = new AudioStreamPlayer2D();
		t.VolumeDb = volumeDb;
		t.Stream = (AudioStream)ResourceLoader.Load(audiofolder + name);
		t.Finished += () => t.QueueFree();
		reference.GetTree().Root.AddChild(t);
		t.Play();
	}

	public static void PlayAndDispose(string name, Node reference)
	{
		var t = new AudioStreamPlayer2D();
		t.Stream = (AudioStream)ResourceLoader.Load(audiofolder + name);
		t.Finished += () => TryDispose(reference);
		reference.GetTree().Root.AddChild(t);
		t.Play();
	}

	public static void PlayAndDispose(string name, float volumeDb, Node reference)
	{
		var t = new AudioStreamPlayer2D();
		t.VolumeDb = volumeDb;
		t.Stream = (AudioStream)ResourceLoader.Load(audiofolder + name);
		t.Finished += () => TryDispose(reference);
		reference.GetTree().Root.AddChild(t);
		t.Play();
	}

    public static void PlayRandomPitch(string name, float pitchvariance, Node reference)
	{
		var t = new AudioStreamPlayer2D();
		t.Stream = (AudioStream)ResourceLoader.Load(audiofolder + name);
		t.Finished += () => t.QueueFree();
		reference.GetTree().Root.AddChild(t);
        RandomNumberGenerator rng = new RandomNumberGenerator();
        var pitch = 1 + rng.RandfRange(-pitchvariance, pitchvariance);
        if (pitch < 0) pitch = 0.1f;
        t.PitchScale = pitch;
		t.Play();
	}

	public static void PlayRandomPitch(string name, float pitchvariance, float volumeDb, Node reference)
	{
		var t = new AudioStreamPlayer2D();
		t.VolumeDb = volumeDb;
		t.Stream = (AudioStream)ResourceLoader.Load(audiofolder + name);
		t.Finished += () => t.QueueFree();
		reference.GetTree().Root.AddChild(t);
        RandomNumberGenerator rng = new RandomNumberGenerator();
        var pitch = 1 + rng.RandfRange(-pitchvariance, pitchvariance);
        if (pitch < 0) pitch = 0.1f;
        t.PitchScale = pitch;
		t.Play();
	}

	public static void PlayRandomPitchAndDispose(string name, float pitchvariance, Node reference)
	{
		var t = new AudioStreamPlayer2D();
		t.Stream = (AudioStream)ResourceLoader.Load(audiofolder + name);
		t.Finished += () => TryDispose(reference);
		reference.GetTree().Root.AddChild(t);
        RandomNumberGenerator rng = new RandomNumberGenerator();
        var pitch = 1 + rng.RandfRange(-pitchvariance, pitchvariance);
        if (pitch < 0) pitch = 0.1f;
        t.PitchScale = pitch;
		t.Play();
	}

	public static void PlayRandomPitchAndDispose(string name, float pitchvariance, float volumeDb, Node reference)
	{
		var t = new AudioStreamPlayer2D();
		t.VolumeDb = volumeDb;
		t.Stream = (AudioStream)ResourceLoader.Load(audiofolder + name);
		t.Finished += () => TryDispose(reference);
		reference.GetTree().Root.AddChild(t);
        RandomNumberGenerator rng = new RandomNumberGenerator();
        var pitch = 1 + rng.RandfRange(-pitchvariance, pitchvariance);
        if (pitch < 0) pitch = 0.1f;
        t.PitchScale = pitch;
		t.Play();
	}

	static void TryDispose(Node n)
	{
		try
		{
			n.QueueFree();
		}
		catch (ObjectDisposedException)
		{

		}
		catch (Exception e)
		{
			GD.PrintErr(e.Message);
		}
	}
}