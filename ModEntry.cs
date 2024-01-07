using System;
using System.IO;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProfitCalculator.App;
using ProfitCalculator.helper;
using ProfitCalculator.menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ProfitCalculator
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
            helper.Events.GameLoop.GameLaunched += onGameLaunchedAddGenericModConfigMenu;
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

        private void onGameLaunchedAddGenericModConfigMenu(object sender, GameLaunchedEventArgs e)
        {
            //register config menu if generic mod config menu is installed
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            // add keybinding setting
            configMenu.AddKeybind(
                mod: this.ModManifest,
                getValue: () => this.Config.HotKey,
                setValue: value => this.Config.HotKey = value,
                name: () => (this.Helper.Translation.Get("open") + " " + this.Helper.Translation.Get("app-name")).ToString(),
                tooltip: () => this.Helper.Translation.Get("hot-key-tooltip")
            );
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