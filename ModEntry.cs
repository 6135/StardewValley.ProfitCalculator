using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MobileProfitCalculator.App;
using MobileProfitCalculator.helper;
using MobileProfitCalculator.menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace MobileProfitCalculator
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        public IMobilePhoneApi api;
        private ModConfig Config;
        private ProfitCalculatorMainMenu mainMenu;
        /*********
        ** Public methods
        *********/

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Helpers.Initialize(helper);
            Monitor.Log($"Helpers initialized", LogLevel.Debug);

            //read config
            Config = Helper.ReadConfig<ModConfig>();
            //hook events
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += onGameLaunched;
        }

        /*********
        ** Private methods
        *********/

        private void onGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            mainMenu = new ProfitCalculatorMainMenu(Helper, Monitor, Config);
            //register app to mobile phone if mobile phone mod is installed
            if (Config.EnableMobileApp)
            {
                api = Helper.ModRegistry.GetApi<IMobilePhoneApi>("aedenthorn.MobilePhone");
                if (api != null)
                {
                    bool success;
                    success = api.AddApp(Helper.ModRegistry.ModID, Helper.Translation.Get("app-name"), OpenApp, Helpers.AppIcon);
                    Monitor.Log($"loaded app successfully: {success}", LogLevel.Debug);
                    ProfitCalculatorApp.Initialize(Helper, Monitor, Config, api);
                }
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            //check if button pressed is button in config
            if (e.Button == Config.HotKey)
            {
                //open menu if not already open else close
                if (!mainMenu.isProfitCalculatorOpen)
                {
                    mainMenu.isProfitCalculatorOpen = true;
                    mainMenu.updateMenu();
                    Game1.activeClickableMenu = mainMenu;
                    Game1.playSound("bigSelect");
                   
                }
                else
                {
                    mainMenu.isProfitCalculatorOpen = false;
                    Game1.activeClickableMenu = null;
                    Game1.playSound("bigDeSelect");
                }
            }
        }

        private void OpenApp()
        {
            Monitor.Log($"Opening App {Helper.ModRegistry.ModID}");
            ProfitCalculatorApp.Start();
        }
    }
}