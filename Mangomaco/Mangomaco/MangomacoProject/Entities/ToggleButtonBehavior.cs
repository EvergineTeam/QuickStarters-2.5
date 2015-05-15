#region Using Statements
using System;
using WaveEngine.Components.Gestures;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics; 
#endregion

namespace MangomacoProject.Entities
{
    /// <summary>
    /// Toggle Button behavior class
    /// </summary>
    public class ToggleButtonBehavior : Behavior
    {
        /// <summary>
        /// Occurs when [Checked].
        /// </summary>
        public event EventHandler CheckedChanged;

        [RequiredComponent]
        public TouchGestures Gestures;

        private Transform2D uncheckedTransform, uncheckedPressedTransform, checkedTransform, checkedPressedTransform;
        private bool isChecked;
        private bool updateOnRefresh;

        #region Properties
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (this.isChecked == value && !this.updateOnRefresh)
                    return;

                this.isChecked = value;

                if (this.uncheckedPressedTransform == null)
                {
                    this.updateOnRefresh = true;
                    return;
                }

                this.updateCheckState();

                if (this.CheckedChanged != null)
                    this.CheckedChanged(this, new EventArgs());
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleButtonBehavior" /> class.
        /// </summary>
        public ToggleButtonBehavior(bool initialChecked)
            : base("toggleButtonBehavior")
        {
            this.isChecked = initialChecked;
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.uncheckedTransform = Owner.FindChild("UncheckedImage").FindComponent<Transform2D>();
            this.uncheckedPressedTransform = Owner.FindChild("UncheckedPressedImage").FindComponent<Transform2D>();
            this.checkedTransform = Owner.FindChild("CheckedImage").FindComponent<Transform2D>();
            this.checkedPressedTransform = Owner.FindChild("CheckedPressedImage").FindComponent<Transform2D>();

            this.Gestures.TouchReleased -= Gestures_TouchReleased;
            this.Gestures.TouchReleased += Gestures_TouchReleased;

            //Force update
            if (this.updateOnRefresh)
            {
                this.IsChecked = this.isChecked;
                this.updateOnRefresh = false;
            }
            else if (this.isChecked)
            {
                updateCheckState();
            }
        }

        /// <summary>
        /// Sets the state of the check.
        /// </summary>
        private void updateCheckState()
        {
            if (this.isChecked)
            {
                this.uncheckedPressedTransform.Opacity = 0;
                this.uncheckedTransform.Opacity = 0;
                this.checkedPressedTransform.Opacity = 0;
                this.checkedTransform.Opacity = 1;
            }
            else
            {
                this.checkedPressedTransform.Opacity = 0;
                this.checkedTransform.Opacity = 0;
                this.uncheckedPressedTransform.Opacity = 0;
                this.uncheckedTransform.Opacity = 1;
            }
        }

        /// <summary>
        /// Handles the TouchReleased event of the Gestures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GestureEventArgs" /> instance containing the event data.</param>
        private void Gestures_TouchReleased(object sender, GestureEventArgs e)
        {
            this.IsChecked = !this.IsChecked;
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if the <see cref="T:WaveEngine.Framework.Component" />, or the <see cref="T:WaveEngine.Framework.Entity" />
        /// owning it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
        }
    }
}