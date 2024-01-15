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
using static ProfitCalculator.Utils;
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
        private readonly List<Vector4> OptionSlots = new();
        private ClickableTextureComponent upArrow;
        private ClickableTextureComponent downArrow;
        private Rectangle scrollBarBounds;
        private ClickableTextureComponent scrollBar;
        private int currentItemIndex = 0;
        private int maxOptions = 6;
        private bool scrolling = false;
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
            for (int i = 0; i < maxOptions; i++)
            {
                OptionSlots.Add(
                    new(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + 10,
                        this.yPositionOnScreen + (spaceToClearTopBorder + 5) + (Game1.tileSize / 2) + ((Game1.tileSize + Game1.tileSize / 2) * i),
                        widthOnScreen - ((spaceToClearSideBorder + borderWidth + 10) * 2),
                        Game1.tileSize
                   )
                );
            }
            foreach (CropInfo cropInfo in cropInfos)
            {
                Options.Add(
                    new CropBox(
                        0,
                        0,
                        0,
                        0,
                        cropInfo
                    )
                );
            }

            behaviorBeforeCleanup = delegate
            {
                IsResultsListOpen = false;
            };

            this.xPositionOnScreen = (int)getAppropriateMenuPosition().X;
            this.yPositionOnScreen = (int)getAppropriateMenuPosition().Y;
            int scrollbar_x = xPositionOnScreen + width + 16;
            this.upArrow = new ClickableTextureComponent(
                new Rectangle(scrollbar_x, yPositionOnScreen + Game1.tileSize + Game1.tileSize / 3, 44, 48),
                Game1.mouseCursors,
                new Rectangle(421, 459, 11, 12),
                4f);
            this.downArrow = new ClickableTextureComponent(
                new Rectangle(scrollbar_x, yPositionOnScreen + height - 64, 44, 48),
                Game1.mouseCursors,
                new Rectangle(421, 472, 11, 12),
                4f);
            this.scrollBarBounds = default;
            this.scrollBarBounds.X = this.upArrow.bounds.X + 12;
            this.scrollBarBounds.Width = 24;
            this.scrollBarBounds.Y = this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4;
            this.scrollBarBounds.Height = this.downArrow.bounds.Y - 4 - this.scrollBarBounds.Y;
            this.scrollBar = new ClickableTextureComponent(new Rectangle(this.scrollBarBounds.X, this.scrollBarBounds.Y, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
            //max options is a division of the height of the menu by the height of each option
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

            for (int i = 0; i < maxOptions; i++)
            {
                if (currentItemIndex + i >= Options.Count)
                    break;
                Options[currentItemIndex + i].ClickableComponent = new(new(
                    (int)OptionSlots[i].X,
                    (int)OptionSlots[i].Y,
                    (int)OptionSlots[i].Z,
                    (int)OptionSlots[i].W
                ), Options[currentItemIndex + i].Name());
                Options[currentItemIndex + i].Draw(b);
            }

            this.upArrow.draw(b);
            this.downArrow.draw(b);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarBounds.X, this.scrollBarBounds.Y, this.scrollBarBounds.Width, this.scrollBarBounds.Height, Color.White, 4f, drawShadow: false);
            this.scrollBar.draw(b);
            // Draw Labels and Options and buttons
            /*this.drawActions(b);
            this.drawLabels(b);
            //print active sort mode from b (private field called _sortMode)
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            this.drawOptions(b);
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);*/

            if (shouldDrawCloseButton())
                base.draw(b);
            if (!Game1.options.hardwareCursor)
                b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }

        #endregion Draw Methods

        #region Event Handling

        private void setScrollBarToCurrentIndex()
        {
            if (this.Options.Count > 0)
            {
                this.scrollBar.bounds.Y = this.scrollBarBounds.Y + this.currentItemIndex * this.scrollBarBounds.Height / Math.Max(1, this.Options.Count - maxOptions / 2);
                /*                if (this.currentItemIndex == this.Options.Count - maxOptions)
                                {
                                    this.scrollBar.bounds.Y = this.downArrow.bounds.Y - this.scrollBar.bounds.Height;
                                }*/
            }
            else
            {
                this.scrollBar.bounds.Y = this.scrollBarBounds.Y;
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && this.currentItemIndex > 0)
            {
                this.arrowPressed(-1);
                Game1.playSound("shiny4");
            }
            else if (direction < 0 && this.currentItemIndex < Math.Max(0, this.Options.Count - maxOptions))
            {
                arrowPressed();
                Game1.playSound("shiny4");
            }
            if (Game1.options.SnappyMenus)
            {
                this.snapCursorToCurrentSnappedComponent();
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            switch (key)
            {
                case Keys.Escape:
                    exitThisMenu();
                    break;

                case Keys.Down:
                    if (currentItemIndex + maxOptions < Options.Count)
                    {
                        arrowPressed();
                        Game1.playSound("shwip");
                    }
                    break;

                case Keys.Up:
                    if (currentItemIndex - maxOptions >= 0)
                    {
                        arrowPressed(-1);
                        Game1.playSound("shwip");
                    }
                    break;
            }
        }

        public override void performHoverAction(int x, int y)
        {
            for (int i = 0; i < this.OptionSlots.Count; i++)
            {
                if (this.currentItemIndex >= 0 && this.currentItemIndex + i < this.Options.Count && this.Options[this.currentItemIndex + i].ClickableComponent.bounds.Contains(x - this.OptionSlots[i].X, y - this.OptionSlots[i].Y))
                {
                    Game1.SetFreeCursorDrag();
                    break;
                }
            }
            if (this.scrollBarBounds.Contains(x, y))
            {
                Game1.SetFreeCursorDrag();
            }
            if (GameMenu.forcePreventClose)
            {
                return;
            }

            this.upArrow.tryHover(x, y);
            this.downArrow.tryHover(x, y);
            this.scrollBar.tryHover(x, y);
            _ = this.scrolling;
        }

        public virtual void SetScrollFromY(int y)
        {
            int y2 = this.scrollBar.bounds.Y;
            float percentage = (float)(y - this.scrollBarBounds.Y) / (float)this.scrollBarBounds.Height;
            this.currentItemIndex = (int)Utility.Lerp(t: Utility.Clamp(percentage, 0f, 1f), a: 0f, b: this.Options.Count - maxOptions);
            this.setScrollBarToCurrentIndex();
            if (y2 != this.scrollBar.bounds.Y)
            {
                Game1.playSound("shiny4");
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            if (!GameMenu.forcePreventClose)
            {
                base.leftClickHeld(x, y);
                if (this.scrolling)
                {
                    this.SetScrollFromY(y);
                }
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (!GameMenu.forcePreventClose)
            {
                base.releaseLeftClick(x, y);
                this.scrolling = false;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (GameMenu.forcePreventClose)
            {
                return;
            }
            if (this.downArrow.containsPoint(x, y) && this.currentItemIndex < Math.Max(0, this.Options.Count - maxOptions))
            {
                this.arrowPressed(1);
                Game1.playSound("shwip");
            }
            else if (this.upArrow.containsPoint(x, y) && this.currentItemIndex > 0)
            {
                this.arrowPressed(-1);
                Game1.playSound("shwip");
            }
            else if (this.scrollBar.containsPoint(x, y))
            {
                this.scrolling = true;
            }
            else if (!this.downArrow.containsPoint(x, y) && x > base.xPositionOnScreen + base.width && x < base.xPositionOnScreen + base.width + 128 && y > base.yPositionOnScreen && y < base.yPositionOnScreen + base.height)
            {
                this.scrolling = true;
                this.leftClickHeld(x, y);
                this.releaseLeftClick(x, y);
            }
        }

        private void arrowPressed(int direction = 1)
        {
            if (direction == 1)
            {
                this.downArrow.scale = this.downArrow.baseScale;
            }
            else
            {
                this.upArrow.scale = this.upArrow.baseScale;
            }
            this.currentItemIndex += direction;
            this.setScrollBarToCurrentIndex();
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