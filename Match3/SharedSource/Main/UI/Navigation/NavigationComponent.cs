using Match3.Services.Navigation;
using System.Runtime.Serialization;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace Match3.UI.Navigation
{
    [DataContract]
    public class NavigationComponent : Component
    {
        [DataMember]
        public NavigateCommands NavigationCommand { get; set; }

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.NavigationCommand = NavigateCommands.Back;
        }

        public void DoNavigation()
        {
            var navService = WaveServices.GetService<NavigationService>();

            if (navService != null)
            {
                navService.Navigate(this.NavigationCommand);
            }
        }
    }
}
