using System.Windows.Threading;

namespace Minesweeper.Core
{
    public interface IMineService : IWindow
    {
        public Dispatcher GetDispatcher();
    }
}
