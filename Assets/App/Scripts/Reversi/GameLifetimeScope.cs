using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace App.Reversi
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private ReversiBoard _reversiBoard;
        [SerializeField] private UiManager _uiManager;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_reversiBoard);
            builder.RegisterComponent(_uiManager);
            builder.Register<ReversiService>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GamePresenter>();
            
            // Provider
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<CellStateParams>(options);
            builder.RegisterMessageBroker<BoardInputParams>(options);
            builder.RegisterEntryPoint<BoardInputProvider>();
        }
    }
}