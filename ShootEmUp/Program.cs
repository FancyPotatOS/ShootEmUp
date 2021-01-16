using System;

namespace ShootEmUp
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new SEU())
                game.Run();
        }
    }
}
