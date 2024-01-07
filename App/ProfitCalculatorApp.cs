using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProfitCalculator.helper;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace ProfitCalculator.App
{
    public class ProfitCalculatorApp
    {
        private static IModHelper helper;
        private static IMonitor monitor;
        private static ModConfig config;
        private static IMobilePhoneApi api;

        private static int xPositionOnScreen;
        private static int yPositionOnScreen;
        private static int widthOnScreen = 0;
        private static int heightOnScreen = 0;
        private static int timer = 0;
        private static Texture2D backgroundTexture;
        private static Texture2D backgroundLandscapeTexture;

        //initialize the app by setting the helper, monitor, config, and api
        internal static void Initialize(IModHelper _helper, IMonitor _monitor, ModConfig _config, IMobilePhoneApi _api)
        {
            helper = _helper;
            monitor = _monitor;
            config = _config;
            api = _api;

            Vector2 ps = api.GetScreenSize(false);
            Vector2 ls = api.GetScreenSize(true);
            backgroundTexture = new Texture2D(Game1.graphics.GraphicsDevice, (int)ps.X, (int)ps.Y);
            backgroundLandscapeTexture = new Texture2D(Game1.graphics.GraphicsDevice, (int)ls.X, (int)ls.Y);
            Color[] data = new Color[backgroundTexture.Width * backgroundTexture.Height];
            Color[] data2 = new Color[backgroundLandscapeTexture.Width * backgroundLandscapeTexture.Height];
            int i = 0;
            while (i < data.Length || i < data2.Length)
            {
                if (i < data.Length)
                    data[i] = config.AppBackgroundColor;
                if (i < data2.Length)
                    data2[i] = config.AppBackgroundColor;
                i++;
            }
            backgroundTexture.SetData(data);
            backgroundLandscapeTexture.SetData(data2);
        }

        internal static void Start()
        {
            api.SetRunningApp(helper.ModRegistry.ModID);
            api.SetAppRunning(true);
            helper.Events.Display.Rendered += Display_Rendered;
            //helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private static void Display_Rendered(object sender, RenderedEventArgs e)
        {
            if (api.IsCallingNPC())
                return;

            if (!api.GetPhoneOpened() || !api.GetAppRunning() || api.GetRunningApp() != helper.ModRegistry.ModID)
            {
                monitor.Log($"Closing app");
                helper.Events.Display.Rendered -= Display_Rendered;
                //helper.Events.Input.ButtonPressed -= Input_ButtonPressed;
                return;
            }

            Rectangle screenRect = api.GetScreenRectangle();
            xPositionOnScreen = screenRect.X + config.AppMarginX;
            yPositionOnScreen = screenRect.Y + config.AppMarginY;
            widthOnScreen = screenRect.Width - config.AppMarginX * 2;
            heightOnScreen = screenRect.Height - config.AppMarginY * 2;

            bool rotated = api.GetPhoneRotated();
            int textHeight = (Game1.smallFont.LineSpacing + config.AppMarginY) * (rotated ? 1 : 2);
            int spaceBelowText = heightOnScreen - textHeight;
            int buttonHeight1 = spaceBelowText / 5;
            int buttonHeight2 = (spaceBelowText - buttonHeight1) / 6;

            SpriteBatch b = e.SpriteBatch;

            b.Draw(rotated ? backgroundLandscapeTexture : backgroundTexture, screenRect, Color.White);

            // Draw JoJa watermark thing.
            b.Draw(
                Helpers.AppIcon,
                new Vector2
                (
                    xPositionOnScreen,
                    yPositionOnScreen
                        + heightOnScreen
                        - buttonHeight1 / 2
                        - Helpers.AppIcon.Height / 2
                ),
                Color.White
            );

            //if (shouldDrawCloseButton()) base.draw(b);
            /*          if (!Game1.options.hardwareCursor) b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);*/
        }

        private static void exitApp()
        {
            if (api.GetRunningApp() == helper.ModRegistry.ModID)
            {
                api.SetAppRunning(false);
                api.SetRunningApp(null);
            }
        }
    }
}