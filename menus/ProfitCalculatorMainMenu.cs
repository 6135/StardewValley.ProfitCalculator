using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MobileProfitCalculator.helper;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using static MobileProfitCalculator.helper.Helpers;

namespace MobileProfitCalculator.menus
{
    public class ProfitCalculatorMainMenu : IClickableMenu
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly ModConfig config;

        //option variables
        //Day int max 28 min 1
        //Season dropdown
        //Produce Type dropdown
        //Fertilizer Quality dropdown
        //Pay for Seeds checkbox
        //Pay for Fertilizer checkbox
        //Max Money int
        //Use Base Stats checkbox

        private int day = 1;
        private Season season = Helpers.Season.Spring;
        private ProduceType produceType = Helpers.ProduceType.Raw;
        private FertilizerQuality fertilizerQuality = Helpers.FertilizerQuality.None;
        private bool payForSeeds = true;
        private bool payForFertilizer = false;
        private int maxMoney = 0; //infinite
        private bool useBaseStats = false;

        public static int widthOnScreen = 632 + borderWidth * 2;
        public static int heightOnScreen = 600 + borderWidth * 2 + Game1.tileSize;
        private int timer = 0;

        private enum Operation
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            None
        }

        private Operation op = Operation.None;
        private double result = 0;
        private string inputA = "";
        private string inputB = "";
        private bool currentInput = false;

        private readonly List<ClickableComponent> Labels = new List<ClickableComponent>();
        private readonly List<ClickableComponent> Options = new List<ClickableComponent>();

        private ClickableComponent calculateButton;
        private ClickableComponent resetButton;
        private Texture2D logo;
        public bool isProfitCalculatorOpen = false;

        public ProfitCalculatorMainMenu(IModHelper _helper, IMonitor _monitor, ModConfig _modConfig) :
            base(
                (int)getAppropriateMenuPosition().X,
                (int)getAppropriateMenuPosition().Y,
                widthOnScreen,
                heightOnScreen)
        {
            helper = _helper;
            monitor = _monitor;
            config = _modConfig;

            behaviorBeforeCleanup = delegate
            {
                isProfitCalculatorOpen = false;
            };

            this.xPositionOnScreen = (int)getAppropriateMenuPosition().X;
            this.yPositionOnScreen = (int)getAppropriateMenuPosition().Y;
            this.updateMenu();
            this.resetMenu();
            logo = Helpers.AppIcon;
        }

        public void updateMenu()
        {
            Labels.Clear();
            Options.Clear();
            //this.resetMenu();
            this.setUpPositions();
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

        private void setUpPositions()
        {
            this.setUpButtonPositions();
            //option order:
            //Day int
            this.setUpDayOptionPositions();
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
            this.setUpBaseStatsOptionPositions();
        }

        private void setUpButtonPositions()
        {
            calculateButton = new ClickableComponent(
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
                );
        }

        private void setUpDayOptionPositions()
        {
            Labels.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                        this.yPositionOnScreen + spaceToClearTopBorder,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "day",
                    helper.Translation.Get("day") + ": "
                )
            );
            Options.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 7 + Game1.tileSize / 4,
                        this.yPositionOnScreen + spaceToClearTopBorder,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "day",
                    day.ToString()
                )
            );
        }

        private void setUpSeasonOptionPositions()
        {
            Labels.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "season",
                    helper.Translation.Get("season") + ": "
                )
            );
            Options.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 7 + Game1.tileSize / 4,
                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "season",
                    season.ToString()
                )
            );
        }

        private void setUpProduceTypeOptionPositions()
        {
            Labels.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 2,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "produceType",
                    helper.Translation.Get("produce-type") + ": "
                )
            );
            Options.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 7 + Game1.tileSize / 4,
                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 2,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "produceType",
                    produceType.ToString()
                )
            );
        }

        private void setUpFertilizerQualityPositions()
        {
            Labels.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 3,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "fertilizerQuality",
                    helper.Translation.Get("fertilizer-type") + ": "
                )
            );
            Options.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 7 + Game1.tileSize / 4,
                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 3,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "fertilizerQuality",
                    fertilizerQuality.ToString()
                )
            );
        }

        private void setUpSeedsOptionPositions()
        {
            Labels.Add(
                               new ClickableComponent(
                                                      new Rectangle(
                                                                                 this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                                                                                                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 4,
                                                                                                                               Game1.tileSize * 2,
                                                                                                                                                      Game1.tileSize
                                                                                                                                                                         ),
                                                                         "payForSeeds",
                                                                                            helper.Translation.Get("pay-for-seeds") + ": "
                                                                                                           )
                                          );
            Options.Add(
                               new ClickableComponent(
                                                      new Rectangle(
                                                                                 this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 7 + Game1.tileSize / 4,
                                                                                                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 4,
                                                                                                                               Game1.tileSize * 2,
                                                                                                                                                      Game1.tileSize
                                                                                                                                                                         ),
                                                                         "payForSeeds",
                                                                                            payForSeeds.ToString()
                                                                                                           )
                                          );
        }

        private void setUpFertilizerOptionPositions()
        {
            Labels.Add(
                                              new ClickableComponent(
                                                                                                       new Rectangle(
                                                                                                                                                                                           this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                                                                                                                                                                                                                                                                                                  this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 5,
                                                                                                                                                                                                                                                                                                                                                                                                                                Game1.tileSize * 2,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     Game1.tileSize
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             ),
                                                                                                                                                                               "payForFertilizer",
                                                                                                                                                                                                                                                                          helper.Translation.Get("pay-for-fertilizer") + ": "
                                                                                                                                                                                                                                                                                                                                                                                    )
                                                                                       );
            Options.Add(
                                              new ClickableComponent(
                                                                                                       new Rectangle(
                                                                                                                                                                                           this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 7 + Game1.tileSize / 4,
                                                                                                                                                                                                                                                                                                  this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 5,
                                                                                                                                                                                                                                                                                                                                                                                                                                Game1.tileSize * 2,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     Game1.tileSize
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             ),
                                                                                                                                                                               "payForFertilizer",
                                                                                                                                                                                                                                                                          payForFertilizer.ToString()
                                                                                                                                                                                                                                                                                                                                                                                    )
                                                                                       );
        }

        private void setUpMoneyOptionPositions()
        {
            Labels.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 6,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "maxMoney",
                    helper.Translation.Get("max-money") + ": "
                )
            );
            Options.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 7 + Game1.tileSize / 4,
                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 6,
                        Game1.tileSize * 2,
                        Game1.tileSize
                        ),
                        "maxMoney",
                        maxMoney.ToString()
                    )
                );
        }

        private void setUpBaseStatsOptionPositions()
        {
            Labels.Add(
                               new ClickableComponent(
                                                      new Rectangle(
                                                                                 this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                                                                                                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 7,
                                                                                                                               Game1.tileSize * 2,
                                                                                                                                                      Game1.tileSize
                                                                                                                                                                         ),
                                                                         "useBaseStats",
                                                                         helper.Translation.Get("base-stats") + ": "
                                                                                                           )
                                          );
            Options.Add(
                               new ClickableComponent(
                                                      new Rectangle(
                                                                                 this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 7 + Game1.tileSize / 4,
                                                                                                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 7,
                                                                                                                               Game1.tileSize * 2,
                                                                                                                                                      Game1.tileSize
                                                                                                                                                                         ),
                                                                         "useBaseStats",
                                                                                            useBaseStats.ToString()
                                                                                                           )
                                          );
        }

        public override void draw(SpriteBatch b)
        {
            if (!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, widthOnScreen, heightOnScreen, speaker: false, drawOnlyBox: true);

            /*// Draw the current input.
            b.DrawString
            (
                Game1.smallFont,
                String.Concat(((currentInput) ? inputB : inputA), ((timer >= 16) ? "|" : "")),
                new Vector2(
                    (float)xPositionOnScreen
                        + widthOnScreen
                        - 40
                        - Game1.smallFont.MeasureString((currentInput) ? inputB + "|" : inputA + "|").X,
                    (float)yPositionOnScreen
                        + IClickableMenu.borderWidth
                        + IClickableMenu.spaceToClearTopBorder
                ),
                Game1.textColor
            );

            // Draw the previous input, if currently entering the second number.
            if (currentInput)
            {
                string prevInput = inputA
                    + " "
                    + (op switch
                    {
                        Operation.Add => "+",
                        Operation.Subtract => "-",
                        Operation.Multiply => "X",
                        Operation.Divide => "/",
                        Operation.None => "ERR",
                        _ => "ERR",
                    });
                b.DrawString
                (
                    Game1.smallFont,
                    prevInput,
                    new Vector2
                    (
                        (float)xPositionOnScreen
                            + widthOnScreen
                            - 40
                            - Game1.smallFont.MeasureString(prevInput).X,
                        (float)yPositionOnScreen
                            + IClickableMenu.borderWidth
                            + IClickableMenu.spaceToClearSideBorder
                            + Game1.smallFont.LineSpacing * 2
                    ),
                    Game1.textShadowColor
                );
            }*/

            // Draw the number pad.
            /*            foreach (ClickableComponent button in numpad)
                        {
                            IClickableMenu.drawTextureBox
                            (
                                b,
                                Game1.mouseCursors,
                                new Rectangle(432, 439, 9, 9),
                                button.bounds.X,
                                button.bounds.Y,
                                button.bounds.Width,
                                button.bounds.Height,
                                (button.scale != 1.0001f) ? Color.Wheat : Color.White,
                                4f,
                                false
                            );
                            Utility.drawBoldText
                            (
                                b,
                                button.name,
                                Game1.smallFont,
                                new Vector2
                                (
                                    (float)button.bounds.X
                                        + (button.bounds.Width / 2)
                                        - (Game1.smallFont.MeasureString(button.name).X / 2),
                                    (float)button.bounds.Y
                                        + (button.bounds.Height / 2)
                                        - (Game1.smallFont.MeasureString(button.name).Y / 2)
                                ),
                                Game1.textColor,
                                1f,
                                -1f,
                                2
                            );
                        }

                        // Draw the operator buttons.
                        foreach (ClickableComponent button in opButtons)
                        {
                            IClickableMenu.drawTextureBox
                            (
                                b,
                                Game1.mouseCursors,
                                new Rectangle(432, 439, 9, 9),
                                button.bounds.X,
                                button.bounds.Y,
                                button.bounds.Width,
                                button.bounds.Height,
                                (button.scale != 1.0001f) ? Color.Wheat : Color.White,
                                4f,
                                false
                            );
                            Utility.drawBoldText
                            (
                                b,
                                button.name,
                                Game1.smallFont,
                                new Vector2(
                                    (float)button.bounds.X
                                        + (button.bounds.Width / 2)
                                        - (Game1.smallFont.MeasureString(button.name).X / 2),
                                    (float)button.bounds.Y
                                        + (button.bounds.Height / 2)
                                        - (Game1.smallFont.MeasureString(button.name).Y / 2)
                                        + 2
                                ),
                                Game1.textColor,
                                1f,
                                -1f,
                                2
                            );
                        }
            */

            // Draw Labels and Options and buttons
            this.drawActions(b);
            this.drawLabels(b);
            this.drawOptions(b);

            //if (shouldDrawCloseButton()) base.draw(b);
            if (!Game1.options.hardwareCursor) b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);

            /*timer++;
            if (timer >= 32) timer = 0;*/
        }

        private void drawActions(SpriteBatch b)
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
                /*               IClickableMenu.drawTextureBox(
                                   b,
                                   Game1.mouseCursors,
                                   new Rectangle(432, 439, 9, 9),
                                   label.bounds.X,
                                   label.bounds.Y,
                                   label.bounds.Width,
                                   label.bounds.Height,
                                   (label.scale != 1.0001f) ? Color.Wheat : Color.White,
                                   4f,
                                   false
                               );*/
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
            foreach (ClickableComponent option in Options)
            {
                /*                IClickableMenu.drawTextureBox(
                                    b,
                                    Game1.mouseCursors,
                                    new Rectangle(432, 439, 9, 9),
                                    option.bounds.X,
                                    option.bounds.Y,
                                    option.bounds.Width,
                                    option.bounds.Height,
                                    (option.scale != 1.0001f) ? Color.Wheat : Color.White,
                                    4f,
                                    false
                                );*/
                b.DrawString(
                    Game1.dialogueFont,
                    option.label,
                    new Vector2(
                        (float)option.bounds.X,
                        (float)option.bounds.Y + (option.bounds.Height / 2) - (Game1.smallFont.MeasureString(option.name).Y / 2)
                    ),
                    Game1.textColor
                );
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            /*char keyChar = key switch
            {
                Keys.NumPad0 => '0',
                Keys.NumPad1 => '1',
                Keys.NumPad2 => '2',
                Keys.NumPad3 => '3',
                Keys.NumPad4 => '4',
                Keys.NumPad5 => '5',
                Keys.NumPad6 => '6',
                Keys.NumPad7 => '7',
                Keys.NumPad8 => '8',
                Keys.NumPad9 => '9',
                Keys.D0 => '0',
                Keys.D1 => '1',
                Keys.D2 => '2',
                Keys.D3 => '3',
                Keys.D4 => '4',
                Keys.D5 => '5',
                Keys.D6 => '6',
                Keys.D7 => '7',
                Keys.D8 => '8',
                Keys.D9 => '9',
                Keys.OemPeriod => '.',
                _ => '_',
            };*/

            switch (key)
            {
                case Keys.Enter:
                    DoCalculation();
                    Game1.playSound("select");
                    break;

                case Keys.Escape:
                    exitThisMenu();
                    break;
            }
        }

        public override void performHoverAction(int x, int y)
        {
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            /*            foreach (ClickableComponent button in numpad)
                        {
                            if (button.containsPoint(x, y))
                            {
                                if (currentInput)
                                    inputB += button.name;
                                else
                                    inputA += button.name;
                                Game1.playSound("smallSelect");
                            }
                        }
                        foreach (ClickableComponent button in opButtons)
                        {
                            if (button.containsPoint(x, y))
                            {
                                switch (button.name)
                                {
                                    case "+":
                                        if (!currentInput)
                                            currentInput = true;
                                        op = Operation.Add;
                                        break;

                                    case "-":
                                        if (!currentInput)
                                            currentInput = true;
                                        op = Operation.Subtract;
                                        break;

                                    case "X":
                                        if (!currentInput)
                                            currentInput = true;
                                        op = Operation.Multiply;
                                        break;

                                    case "/":
                                        if (!currentInput)
                                            currentInput = true;
                                        op = Operation.Divide;
                                        break;

                                    case "EQ":
                                        DoCalculation();
                                        break;

                                    case ".":
                                        if (!currentInput)
                                            inputA += ".";
                                        else
                                            inputB += ".";
                                        break;
                                }
                                Game1.playSound("smallSelect");
                            }
                        }
                        if (zeroButton.containsPoint(x, y))
                        {
                            if (!currentInput)
                                inputA += "0";
                            else
                                inputB += "0";
                            Game1.playSound("smallSelect");
                        }*/
            if (calculateButton.containsPoint(x, y))
            {
                this.DoCalculation();
                if (playSound) Game1.playSound("select");
            }
            if (resetButton.containsPoint(x, y))
            {
                this.resetMenu();
                if (playSound) Game1.playSound("dialogueCharacterClose");
            }
        }

        private void resetMenu()
        {
            //set all the options to default values
            //get day from game
            day = Game1.dayOfMonth;
            season = (Season)Season.Parse(typeof(Season), Game1.currentSeason, true);
            produceType = Helpers.ProduceType.Raw;
            fertilizerQuality = Helpers.FertilizerQuality.None;
            payForSeeds = true;
            payForFertilizer = false;
            maxMoney = Game1.player.team.money.Value;
            useBaseStats = false;
        }

        private void DoCalculation()
        {
            monitor.Log("Doing Calculation", LogLevel.Debug);
        }
    }
}