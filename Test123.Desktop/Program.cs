using osu.Framework.Platform;
using osu.Framework;
using Test123.Game;

namespace Test123.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableDesktopHost(@"Test123"))
            using (osu.Framework.Game game = new Test123Game())
                host.Run(game);
        }
    }
}
