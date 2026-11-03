using BepInEx;
using BepInEx.Logging;
using ImprovedInput;
using On.HUD;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using UnityEngine;

// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618


namespace MonologSkip;

[BepInPlugin("NV.DialogSkip", "Dialog skip", "2.0.0")]
[BepInDependency("com.dual.improved-input-config", BepInDependency.DependencyFlags.SoftDependency)]
sealed class Plugin : BaseUnityPlugin
{
    const int framespersecond = 40;
    const int cooldown = (int)(framespersecond * 0.2f);
    int counter = 0;
    public new static ManualLogSource Logger;
    static bool IsInit;
    public static class IIC_E_Compatibility
    {
        private static bool? _enabled;
        public static object SkipMonolog;
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

    public void OnEnable()
    {
        Logger = base.Logger;
        On.RainWorld.OnModsInit += OnModsInit;
    }

    void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        if (IsInit) return;
        IsInit = true;

        if (IIC_E_Compatibility.Enabled)
        {
            IIC_E_Compatibility.Initialize();
        }

        DialogBox.Update += DialogBoxOnUpdate;
    }

    void DialogBoxOnUpdate(DialogBox.orig_Update orig, HUD.DialogBox self)
    {
        
        orig(self);
        if (self.hud.owner is Player player && 
            player.abstractCreature.world.game.Players.Any(abstractPlayer => 
            abstractPlayer.realizedCreature is Player p2 && 
            (IIC_E_Compatibility.Enabled ? IIC_E_Compatibility.CheckRawPressed(p2.playerState.playerNumber) : p2.input[0].jmp) && 
            counter == 0))
        {
            if (self.CurrentMessage != null)
            {
                self.showCharacter = self.CurrentMessage.text.Length;
            }
            self.lingerCounter = 10000;
            counter = cooldown;
        }

        if (counter > 0) counter--;
    }
}
