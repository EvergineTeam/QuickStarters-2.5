using System;
using System.Runtime.Serialization;
using WaveEngine.Components.Gestures;
using WaveEngine.Framework;

namespace Match3.UI.Navigation
{
    [DataContract]
    public class TouchNavigationComponent : Component, IDisposable
    {
        [RequiredComponent]
        protected NavigationComponent navComponent;

        [RequiredComponent]
        protected TouchGestures touchGestures;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.touchGestures.TouchTap -= this.TouchGestures_TouchTap;
            this.touchGestures.TouchTap += this.TouchGestures_TouchTap;
        }

        protected override void DeleteDependencies()
        {
            this.RemoveCustomDependencies();

            base.DeleteDependencies();
        }

        public void Dispose()
        {
            this.RemoveCustomDependencies();
        }

        private void RemoveCustomDependencies()
        {
            if (this.touchGestures != null)
            {
                this.touchGestures.TouchTap -= this.TouchGestures_TouchTap;
            }
        }

        private void TouchGestures_TouchTap(object sender, GestureEventArgs e)
        {
            this.navComponent.DoNavigation();
        }
    }
}
