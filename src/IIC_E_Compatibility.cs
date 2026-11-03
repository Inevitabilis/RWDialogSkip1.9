using ImprovedInput;
using System.Runtime.CompilerServices;
using UnityEngine;


namespace MonologSkip;

public static class IIC_E_Compatibility
{
	static bool? _enabled;
	static object SkipMonolog;
	public static bool Enabled
	{
		get
		{
			_enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.dual.improved-input-config");
			return (bool)_enabled;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	public static void Initialize()
	{
		SkipMonolog = PlayerKeybind.Register(
			"DialogSkip:skip", 
			"Dialog Skip", 
			"Skip line", 
			KeyCode.None, 
			KeyCode.None, 
			KeyCode.None);
	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	public static bool CheckRawPressed(int playerNumber)
	{
		if (SkipMonolog == null) return false;
		
		var keybind = (PlayerKeybind)SkipMonolog;
		return keybind.CheckRawPressed(playerNumber);
	}
}