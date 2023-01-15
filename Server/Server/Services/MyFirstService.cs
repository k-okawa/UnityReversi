using App.Reversi.Shared;
using MagicOnion;
using MagicOnion.Server;

namespace UnityReversi.Server.Services
{
    public class MyFirstService : ServiceBase<IMyFirstService>, IMyFirstService
    {
        public async UnaryResult<int> SumAsync(int x, int y)
        {
            Console.WriteLine($"Received:{x}, {y}");
            return x + y;
        }
    }
}