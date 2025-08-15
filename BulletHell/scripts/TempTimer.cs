using System;
using Godot;

public static class TempTimer
{
    /*////////////////////////////////////////////////////////////////////////////////
	
		TEMP TIMER CLASS

		Purpose: Quickly make a self-disposing One-shot timer (mostly because I was kinda tired of making so many timers in my code)

		What was Learned:
			Timers repeat indefinetely by default! I had my ShotCooldown timer repeating forever and didn't even notice
                Except of course if you set OneShot to true, which is VERY important and I overlooked it many times
            _Ready is called whenever you add the node to a scene, so if you want to set it's properties in code,
                should do it before adding as a child

	*/////////////////////////////////////////////////////////////////////////////////
    public static Timer Get(float WaitTime)
    {
        if (WaitTime < 0) WaitTime += 0.001f;
        Timer tt = new Timer()
        {
            Autostart = true,
            OneShot = true,
            WaitTime = WaitTime
        };
        tt.Timeout += tt.QueueFree;
        return tt;
    }
}