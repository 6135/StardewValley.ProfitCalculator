using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProfitCalculator.main;
using ProfitCalculator.ui;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using static ProfitCalculator.Utils;

namespace ProfitCalculator.menus
{
    public class ProfitCalculatorMainMenu : IClickableMenu
    {
        private readonly IModHelper helper;

        private readonly IMonitor monitor;
        private readonly ModConfig config;

        public uint Day { get; set; } = 1;

        public uint MaxDay { get; set; } = 28;
        public uint MinDay { get; set; } = 1;
        public Season Season { get; set; } = Utils.Season.Spring;

        public void setSeason(string season)
        {
            Season = (Season)Season.Parse(typeof(Season), season, false);
        }

        public ProduceType ProduceType { get; set; } = Utils.ProduceType.Raw;
        public FertilizerQuality FertilizerQuality { get; set; } = Utils.FertilizerQuality.None;
        public bool PayForSeeds { get; set; } = true;
        public bool PayForFertilizer { get; set; } = false;
        public uint MaxMoney { get; set; } = 0;
        public bool UseBaseStats { get; set; } = false;

        public bool CrossSeason { get; set; } = true;

        public string exampleString { get; set; } = "example";
        private static int widthOnScreen = 632 + borderWidth * 2;
        private static int heightOnScreen = 600 + borderWidth * 2 + Game1.tileSize;
        private bool stopSpreadingClick = false;

        private readonly List<ClickableComponent> Labels = new List<ClickableComponent>();

        private readonly List<BaseOption> Options = new List<BaseOption>();

        private ClickableComponent calculateButton;
        private ClickableComponent resetButton;
        public bool isProfitCalculatorOpen { get; set; } = false;

        public ProfitCalculatorMainMenu(IModHelper _helper, IMonitor _monitor, ModConfig _modConfig) :
            base(
                (int)GetAppropriateMenuPosition().X,
                (int)GetAppropriateMenuPosition().Y,
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

            this.xPositionOnScreen = (int)GetAppropriateMenuPosition().X;
            this.yPositionOnScreen = (int)GetAppropriateMenuPosition().Y;
            this.ResetMenu();
        }

        #region Menu Button Setups

        private void SetUpPositions()
        {
            this.SetUpButtonPositions();
            //option order:
            //Day int

            this.SetUpDayOptionPositions();
            //Season dropdown
            this.SetUpSeasonOptionPositions();
            //Produce Type dropdown
            this.SetUpProduceTypeOptionPositions();
            //Fertilizer Quality dropdown
            this.SetUpFertilizerQualityPositions();
            //Pay for Seeds checkbox
            this.SetUpSeedsOptionPositions();
            //Pay for Fertilizer checkbox
            this.SetUpFertilizerOptionPositions();
            //Max Money int
            this.SetUpMoneyOptionPositions();
            //Use Base Stats checkbox
            this.SetUpBaseStatsOptionPositions();
        }

        private void SetUpButtonPositions()
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

        private void SetUpDayOptionPositions()
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
            UIntOption dayOption =
               new(
                   this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 5 - Game1.tileSize / 8,
                   this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize / 4,
                   () => "day",
                   () => helper.Translation.Get("day"),
                   valueGetter: () => this.Day,
                   max: () => this.MaxDay,
                   min: () => this.MinDay,
                   valueSetter: (string value) => this.Day = uint.Parse(value),
                   enableClamping: true
               );
            dayOption.setTexture(Helper.ModContent.Load<Texture2D>(Path.Combine("assets", "text_box_small.png")));
            Options.Add(dayOption);
        }

        private void SetUpSeasonOptionPositions()
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

            DropdownOption seasonOption = new(
                this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 5,
                this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize + Game1.tileSize / 4,
                name: () => "season",
                label: () => helper.Translation.Get("season"),
                choices: () => Utils.Season.GetNames(typeof(Utils.Season)),
                labels: () => Utils.GetAllTranslatedSeasons(),
                valueGetter: this.Season.ToString,
                valueSetter:
                    (string value) => this.Season = (Season)Utils.Season.Parse(typeof(Season), value, true)
            )
            {
                MaxValuesAtOnce = Enum.GetValues(typeof(Season)).Length//size of enum
            };

            Options.Add(seasonOption);
        }

        private void SetUpProduceTypeOptionPositions()
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

            DropdownOption produceTypeOption = new(
                this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 5,
                this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 2 + Game1.tileSize / 4,
                name: () => "produceType",
                label: () => helper.Translation.Get("produce-type"),
                choices: () => Utils.ProduceType.GetNames(typeof(Utils.ProduceType)),
                labels: () => Utils.GetAllTranslatedProduceTypes(),
                valueGetter: this.ProduceType.ToString,
                valueSetter: (string value) => this.ProduceType = (ProduceType)Utils.ProduceType.Parse(typeof(ProduceType), value, true)
            )
            {
                MaxValuesAtOnce = Enum.GetValues(typeof(ProduceType)).Length//size of enum
            };
            Options.Add(produceTypeOption);
        }

        private void SetUpFertilizerQualityPositions()
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
            DropdownOption fertilizerQualityOption = new(
                this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 5,
                this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 3 + Game1.tileSize / 4,
                name: () => "fertilizerQuality",
                label: () => helper.Translation.Get("fertilizer-quality"),
                choices: () => Utils.FertilizerQuality.GetNames(typeof(Utils.FertilizerQuality)),
                labels: () => Utils.GetAllTranslatedFertilizerQualities(),
                valueGetter: this.FertilizerQuality.ToString,
                valueSetter: (string value) => this.FertilizerQuality = (FertilizerQuality)Utils.FertilizerQuality.Parse(typeof(FertilizerQuality), value, true)
            )
            {
                MaxValuesAtOnce = Enum.GetValues(typeof(FertilizerQuality)).Length//size of enum
            };
            Options.Add(fertilizerQualityOption);
        }

        private void SetUpSeedsOptionPositions()
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

            CheckboxOption payForSeeds = new(
                    this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 5,
                    this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 4 + Game1.tileSize / 4,
                    () => "payForSeeds",
                    () => helper.Translation.Get("pay-for-seeds"),
                    () => this.PayForSeeds,
                    (bool value) => this.PayForSeeds = value

                );
            Options.Add(payForSeeds);
        }

        private void SetUpFertilizerOptionPositions()
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
            CheckboxOption payForFertilizer = new(
                this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 5,
                this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 5 + Game1.tileSize / 4,
                () => "payForFertilizer",
                () => helper.Translation.Get("pay-for-fertilizer"),
                () => this.PayForFertilizer,
                (bool value) => this.PayForFertilizer = value
            );
            Options.Add(payForFertilizer);
        }

        private void SetUpMoneyOptionPositions()
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
            UIntOption maxMoneyOption =
                new(
                   this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 5 - Game1.tileSize / 8,
                   this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 6 + Game1.tileSize / 4,
                   () => "maxMoney",
                   () => helper.Translation.Get("max-money"),
                   valueGetter: () => this.MaxMoney,
                   max: () => 99999999,
                   min: () => 0,
                   valueSetter: (string value) => this.MaxMoney = uint.Parse(value),
                   enableClamping: true
                );
            Options.Add(maxMoneyOption);
        }

        private void SetUpBaseStatsOptionPositions()
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
            CheckboxOption useBaseStatsOptions = new(
                this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 5,
                this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 7 + Game1.tileSize / 4,
                () => "useBaseStats",
                () => helper.Translation.Get("base-stats"),
                () => this.UseBaseStats,
                (bool value) => this.UseBaseStats = value
            );
            Options.Add(useBaseStatsOptions);
            /*Labels.Add(
                new ClickableComponent(
                    new Rectangle(
                        this.xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                        this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 7,
                        Game1.tileSize * 2,
                        Game1.tileSize
                    ),
                    "crossSeason",
                    helper.Translation.Get("cross-season") + ": "
                )
            );
            CheckboxOption crossSeason = new(
                this.xPositionOnScreen + spaceToClearSideBorder + borderWidth + Game1.tileSize * 5,
                this.yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize * 7 + Game1.tileSize / 4,
                () => "crossSeason",
                () => helper.Translation.Get("cross-season"),
                () => this.CrossSeason,
                (bool value) => this.CrossSeason = value
            );
            Options.Add(crossSeason);*/
        }

        #endregion Menu Button Setups

        #region Draw Methods

        /// <inheritdoc/>
        public override void draw(SpriteBatch b)

        {
            //draw bottom up

            if (!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, widthOnScreen, heightOnScreen, speaker: false, drawOnlyBox: true);

            // Draw Labels and Options and buttons
            this.DrawActions(b);
            this.DrawLabels(b);
            //print active sort mode from b (private field called _sortMode)
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            this.DrawOptions(b);
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            if (shouldDrawCloseButton()) base.draw(b);
            if (!Game1.options.hardwareCursor) b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }

        private void DrawActions(SpriteBatch b)
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

        private void DrawLabels(SpriteBatch b)
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

        private void DrawOptions(SpriteBatch b)
        {
            foreach (BaseOption option in Options)
            {
                option.Draw(b);
            }
        }

        #endregion Draw Methods

        #region Event Handling

        /// <inheritdoc/>
        public override void receiveKeyPress(Keys key)
        {
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

        /// <inheritdoc/>
        public override void performHoverAction(int x, int y)
        {
            //TODO: add hover actions for buttons
        }

        /// <inheritdoc/>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (calculateButton.containsPoint(x, y))
            {
                this.DoCalculation();
                if (playSound) Game1.playSound("select");
                return;
            }
            if (resetButton.containsPoint(x, y))
            {
                this.ResetMenu();
                if (playSound) Game1.playSound("dialogueCharacterClose");
                return;
            }
            //for each option, check if it was clicked
            foreach (BaseOption option in Options)
            {
                if (!stopSpreadingClick)
                    option.ReceiveLeftClick(x, y, () => this.stopSpreadingClick = !this.stopSpreadingClick);
                else
                {
                    this.stopSpreadingClick = !this.stopSpreadingClick;
                    return;
                }
            }
        }

        private void ResetMenu()
        {
            //set all the options to default values
            //get day from game
            Day = (uint)Game1.dayOfMonth;
            Season = (Season)Season.Parse(typeof(Season), Game1.currentSeason, true);
            ProduceType = Utils.ProduceType.Raw;
            FertilizerQuality = Utils.FertilizerQuality.None;
            PayForSeeds = true;
            PayForFertilizer = false;
            MaxMoney = (uint)Game1.player.team.money.Value;
            UseBaseStats = false;
            CrossSeason = true;
            this.UpdateMenu();
        }

        public void UpdateMenu()
        {
            Labels.Clear();
            Options.Clear();
            this.SetUpPositions();
        }

        /// <inheritdoc/>
        public override void update(GameTime time)
        {
            base.update(time);
            //update all the options and labels and buttons
            foreach (BaseOption option in Options)
            {
                option.Update();
            }
        }

        public static Vector2 GetAppropriateMenuPosition()
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

        ///<inheritdoc/>
        /// <summary>The method called when the game window changes size.</summary>
        /// <param name="oldBounds">The former viewport.</param>
        /// <param name="newBounds">The new viewport.</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.xPositionOnScreen = (int)GetAppropriateMenuPosition().X;
            this.yPositionOnScreen = (int)GetAppropriateMenuPosition().Y;

            this.UpdateMenu();
        }

        #endregion Event Handling

        private void DoCalculation()
        {
            ModEntry.Calculator.SetSettings(Day, MaxDay, MinDay, Season, ProduceType, FertilizerQuality, PayForSeeds, PayForFertilizer, MaxMoney, UseBaseStats, CrossSeason);

            /*Dictionary<int, string> crops = Game1.content.Load<Dictionary<int, string>>(@"Data\Crops");
            monitor.Log("----------------------------Data\\Crops----------------------------", LogLevel.Debug);
            //order keyvaluepair by key
            List<KeyValuePair<int, string>> cropsList = new List<KeyValuePair<int, string>>(crops);
            cropsList.Sort(
                delegate (KeyValuePair<int, string> pair1, KeyValuePair<int, string> pair2)
                {
                    Item item = new SObject(pair1.Key, 1).getOne();
                    Item item2 = new SObject(pair2.Key, 1).getOne();
                    return item.Name.CompareTo(item2.Name);
                }
            );
            foreach (KeyValuePair<int, string> crop in cropsList)
            {
                Item item = new SObject(crop.Key, 1);
                monitor.Log($"{item.Name} {item.salePrice()} {item}: {crop.Key}", LogLevel.Debug);
            }*/

            /*if (Helpers.JApi != null)
            {
                monitor.Log("----------------------------JApi----------------------------", LogLevel.Debug);
                foreach (KeyValuePair<string, int> crop in Helpers.JApi.GetAllCropIds())
                {
                    //monitor.Log($"{crop.Key} : {crop.Value}", LogLevel.Debug);
                }
            }
            if (Helpers.DApi != null)
            {
                monitor.Log("----------------------------DApi----------------------------", LogLevel.Debug);
                foreach (string item in Helpers.DApi.GetAllItems())
                {
                    //spawn item as object and get name
                    SObject obj = (SObject)Helpers.DApi.SpawnDGAItem(item);
                    //monitor.Log($"{obj?.GetType()} {obj?.Category} {item}", LogLevel.Debug);
                }
            }*/
            monitor.Log("Doing Calculation", LogLevel.Debug);
            //ModEntry.Calculator.Calculate();
            ModEntry.Calculator.RetrieveCropsAsOrderderList();
            List<CropInfo> cropList = ModEntry.Calculator.RetrieveCropInfos();

            ProfitCalculatorResultsList profitCalculatorResultsList = new ProfitCalculatorResultsList(helper, monitor, config, cropList);
            //Game1.activeClickableMenu = profitCalculatorResultsList;
            this.SetChildMenu(profitCalculatorResultsList);
        }
    }
}