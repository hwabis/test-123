using osu.Framework;
using osu.Framework.Platform;

namespace Test123.Game.Tests
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableDesktopHost("visual-tests"))
            using (var game = new Test123TestBrowser())
                host.Run(game);
        }
    }
}
