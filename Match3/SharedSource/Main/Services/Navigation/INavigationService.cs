namespace Match3.Services.Navigation
{
    public interface INavigationService<TCommands> where TCommands : struct
    {
        bool IsPerformingNavigation { get; }

        void StartNavigation();
        void Navigate(TCommands command);
        bool CanNavigate(TCommands command);
    }
}
