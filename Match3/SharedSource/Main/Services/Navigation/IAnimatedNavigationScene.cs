using WaveEngine.Framework.Services;

namespace Match3.Services.Navigation
{
    public interface IAnimatedNavigationScene
    {
        IGameAction CreateAppearGameAction();
        IGameAction CreateDiappearGameAction();
    }

}
