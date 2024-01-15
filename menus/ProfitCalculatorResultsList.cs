using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using ProfitCalculator.UI;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProfitCalculator.Helpers;
using ProfitCalculator.main;

namespace ProfitCalculator.menus
{
    internal class ProfitCalculatorResultsList : IClickableMenu
    {
        private readonly IModHelper helper;

        private readonly IMonitor monitor;
        private readonly ModConfig config;

        private static int widthOnScreen = 632 + borderWidth * 2;
        private static int heightOnScreen = 600 + borderWidth * 2 + Game1.tileSize;

        private readonly List<CropInfo> cropInfos;
        private readonly List<BaseOption> Options = new();

        public bool IsResultsListOpen { get; set; } = false;

        public ProfitCalculatorResultsList(IModHelper _helper, IMonitor _monitor, ModConfig _modConfig, List<CropInfo> _cropInfos) :
            base(
                (int)getAppropriateMenuPosition().X,
                (int)getAppropriateMenuPosition().Y,
                widthOnScreen,
                heightOnScreen
            )
        {
            helper = _helper;
            monitor = _monitor;
            config = _modConfig;
            cropInfos = _cropInfos;

            behaviorBeforeCleanup = delegate
            {
                IsResultsListOpen = false;
            };

            this.xPositionOnScreen = (int)getAppropriateMenuPosition().X;
            this.yPositionOnScreen = (int)getAppropriateMenuPosition().Y;
        }

        #region Menu Button Setups

        private void setUpPositions()
        {
            this.setUpButtonPositions();
            //option order:
            //Day int

            /*            this.setUpDayOptionPositions();
                        //Season dropdown
                        this.setUpSeasonOptionPositions();
                        //Produce Type dropdown
                        this.setUpProduceTypeOptionPositions();
                        //Fertilizer Quality dropdown
                        this.setUpFertilizerQualityPositions();
                        //Pay for Seeds checkbox
                        this.setUpSeedsOptionPositions();
                        //Pay for Fertilizer checkbox
                        this.setUpFertilizerOptionPositions();
                        //Max Money int
                        this.setUpMoneyOptionPositions();
                        //Use Base Stats checkbox
                        this.setUpBaseStatsOptionPositions();*/
        }

        private void setUpButtonPositions()
        {
            /*            calculateButton = new ClickableComponent(
                            new Rectangle(
                                this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                                this.yPositionOnScreen + borderWidth * 2 + spaceToClearTopBorder + Game1.tileSize * 7,
                                Game1.tileSize * 2,
                                Game1.tileSize
                            ),
                            "calculate",
                            helper.Translation.Get("calculate")
                        );

                        resetButton = new ClickableComponent(
                            new Rectangle(
                                this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 2 + Game1.tileSize / 4,
                                this.yPositionOnScreen + borderWidth * 2 + spaceToClearTopBorder + Game1.tileSize * 7,
                                Game1.tileSize * 2,
                                Game1.tileSize
                            ),
                            "reset",
                            helper.Translation.Get("reset")
                            );*/
        }

        private void setUpMoneyOptionPositions()
        {
        }

        #endregion Menu Button Setups

        #region Draw Methods

        public override void draw(SpriteBatch b)

        {
            //draw bottom up

            if (!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, widthOnScreen, heightOnScreen, speaker: false, drawOnlyBox: true);

            // Draw Labels and Options and buttons
            /*this.drawActions(b);
            this.drawLabels(b);
            //print active sort mode from b (private field called _sortMode)
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            this.drawOptions(b);
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);*/

            if (shouldDrawCloseButton()) base.draw(b);
            if (!Game1.options.hardwareCursor) b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }

        /*private void drawActions(SpriteBatch b)
        {
            // Draw the calculate button.
            IClickableMenu.drawTextureBox
            (
                b,
                Game1.mouseCursors,
                new Rectangle(432, 439, 9, 9),
                calculateButton.bounds.X,
                calculateButton.bounds.Y,
                calculateButton.bounds.Width,
                calculateButton.bounds.Height,
                (calculateButton.scale != 1.0001f) ? Color.Wheat : Color.White,
                4f,
                false
            );
            b.DrawString
            (
                Game1.smallFont,
                calculateButton.label,
                new Vector2
                (
                    (float)calculateButton.bounds.X
                        + (calculateButton.bounds.Width / 2)
                        - (Game1.smallFont.MeasureString(calculateButton.label).X / 2),
                    (float)calculateButton.bounds.Y
                        + (calculateButton.bounds.Height / 2)
                        - (Game1.smallFont.MeasureString(calculateButton.name).Y / 2)
                ),
                Game1.textColor
            );

            // Draw the reset button.
            IClickableMenu.drawTextureBox
            (
                b,
                Game1.mouseCursors,
                new Rectangle(432, 439, 9, 9),
                resetButton.bounds.X,
                resetButton.bounds.Y,
                resetButton.bounds.Width,
                resetButton.bounds.Height,
                (resetButton.scale != 1.0001f) ? Color.Wheat : Color.White,
                4f,
                false
            );
            b.DrawString
            (
                Game1.smallFont,
                resetButton.label,
                new Vector2
                (
                    (float)resetButton.bounds.X
                        + (resetButton.bounds.Width / 2)
                        - (Game1.smallFont.MeasureString(resetButton.label).X / 2),
                    (float)resetButton.bounds.Y
                        + (resetButton.bounds.Height / 2)
                        - (Game1.smallFont.MeasureString(resetButton.name).Y / 2)
                ),
                Game1.textColor
            );
        }

        private void drawLabels(SpriteBatch b)
        {
            foreach (ClickableComponent label in Labels)
            {
                b.DrawString(
                    Game1.dialogueFont,
                    label.label,
                    new Vector2(
                        (float)label.bounds.X,
                        (float)label.bounds.Y + (label.bounds.Height / 2) - (Game1.smallFont.MeasureString(label.name).Y / 2)
                    ),
                    Game1.textColor
                );
            }
        }

        private void drawOptions(SpriteBatch b)
        {
            foreach (BaseOption option in Options)
            {
                option.Draw(b);
            }
        }*/

        #endregion Draw Methods

        #region Event Handling

        public override void receiveKeyPress(Keys key)
        {
            switch (key)
            {
                case Keys.Escape:
                    exitThisMenu();
                    break;
            }
        }

        public override void performHoverAction(int x, int y)
        {
            //TODO: add hover actions for buttons
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
        }

        public void updateMenu()
        {
            //Labels.Clear();
            //Options.Clear();
            this.setUpPositions();
        }

        public override void update(GameTime time)
        {
            base.update(time);
            //update all the options and labels and buttons
            foreach (BaseOption option in Options)
            {
                option.Update();
            }
        }

        public static Vector2 getAppropriateMenuPosition()
        {
            Vector2 defaultPosition = new Vector2(Game1.viewport.Width / 2 - widthOnScreen / 2, (Game1.viewport.Height / 2 - heightOnScreen / 2));

            //Force the viewport into a position that it should fit into on the screen???
            if (defaultPosition.X + widthOnScreen > Game1.viewport.Width)
            {
                defaultPosition.X = 0;
            }

            if (defaultPosition.Y + heightOnScreen > Game1.viewport.Height)
            {
                defaultPosition.Y = 0;
            }
            return defaultPosition;
        }

        /// <summary>The method called when the game window changes size.</summary>
        /// <param name="oldBounds">The former viewport.</param>
        /// <param name="newBounds">The new viewport.</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.xPositionOnScreen = (int)getAppropriateMenuPosition().X;
            this.yPositionOnScreen = (int)getAppropriateMenuPosition().Y;

            this.updateMenu();
        }

        #endregion Event Handling
    }
}