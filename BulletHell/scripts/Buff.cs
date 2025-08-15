using Godot;
using System;
using System.Collections.Generic;

public partial class Buff : Node, IPickup
{
    /*////////////////////////////////////////////////////////////////////////////////
	
		BUFF CLASS

		Purpose: Store multiplicative and flat values for a specific status category

		What was Learned:
			Enums are really good for clarity
            This was kind of simple, but the "Buff Application logic" is actually within the Player class
                which idk is the best practice (Of course, in this game only the player can be buffed)
                if I was doing a game where a Body class had all of these attributes, i'd prob apply the status
                right here, and just go through it in the Body class

	*/////////////////////////////////////////////////////////////////////////////////

    public enum BuffCategory
    {
        Damage,
        FireRate,
        Health,
        Speed,
        Crit,
        Uber
    }

    public enum BuffAddType
    {
        Additive, // 1 + (buff1, buff2, buff3...)
        Multiplicative // (1 + buff1) * (1 + buff2) * (1 + buff3)...
    }

    public BuffCategory Category;
    public float Multiplier = 0; // I have no idea what a negative multiplier would be
    public float Flat = 0;

    protected Timer DurationTimer;
    public float Duration;
    public float DurationLeft 
    {
        get 
        {
            return (float)DurationTimer.TimeLeft;
        }
    }
    public Action Timeout
    {
        set { DurationTimer.Timeout += value; }
    }

    public override void _Ready()
    {
        DurationTimer = new Timer();
        DurationTimer.WaitTime = Duration;
        DurationTimer.OneShot = true;
        AddChild(DurationTimer);
    }

    public void StartTimer()
    {
        if (DurationTimer == null)
        {
            DurationTimer = new Timer();
            DurationTimer.WaitTime = Duration;
            DurationTimer.Autostart = true;
            AddChild(DurationTimer);
        }
        DurationTimer.Start();
    }

    public Buff() {}
    public Buff(BuffCategory category, float duration)
    {
        this.Category = category;
        this.Duration = duration;
    }

    /// <summary>
    /// Returns a List of Buffs of the specified Category
    /// </summary>
    public static List<Buff> FilterBuffs(List<Buff> buffs, BuffCategory Category)
    {
        var res = new List<Buff>();
        foreach (var buff in buffs)
        {
            if (buff.Category == Category)
                res.Add(buff);
        }
        return res;
    }

    /// <summary>
    /// Returns a float of the sum of all flat buffs of a specified Category in the list
    /// </summary>
    public static float GetFlatBuffs(List<Buff> buffs, BuffCategory Category)
    {
        var filtered = FilterBuffs(buffs, Category);
        return GetFlatBuffs(filtered);
    }

    /// <summary>
    /// Returns a float of the sum of all flat buffs in the list
    /// </summary>
    public static float GetFlatBuffs(List<Buff> buffs)
    {
        float res = 0;
        foreach (var buff in buffs)
        {
            res += buff.Flat;
        }
        return res;
    }

    /// <summary>
    /// Returns a float of the sum of all multiplier buffs in the list, from the specific Category and using the specific AddType
    /// </summary>
    public static float GetBuffsMultiplier(List<Buff> buffs, BuffCategory Category, BuffAddType type)
    {
        var filtered = FilterBuffs(buffs, Category);
        return GetBuffsMultiplier(filtered, type); // ADDITIVE BY DEFAULT
    }

    /// <summary>
    /// Returns a float of the sum of all multiplier buffs in the list, from the specific Category. (Additive by default)
    /// </summary>
    public static float GetBuffsMultiplier(List<Buff> buffs, BuffCategory Category)
    {
        var filtered = FilterBuffs(buffs, Category);
        return GetBuffsMultiplier(filtered, BuffAddType.Additive); // ADDITIVE BY DEFAULT
    }

    /// <summary>
    /// Returns a float of the sum of all multiplier buffs in the list, using the specific AddType
    /// </summary>
    public static float GetBuffsMultiplier(List<Buff> buffs, BuffAddType type)
    {
        float res = 1;
        if (type == BuffAddType.Additive)
        {
            foreach (var buff in buffs)
                res += buff.Multiplier;
        }
        else if (type == BuffAddType.Multiplicative)
        {
            foreach (var buff in buffs)
                res *= 1 + buff.Multiplier;
        }
        return res;
    }
}