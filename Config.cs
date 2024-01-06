using StardewModdingAPI;

namespace MobileProfitCalculator
{
    internal class Config
    {
        public SButton debugKey { get; set; }

        public Config()
        {
            debugKey = SButton.J;
        }
    }
}
