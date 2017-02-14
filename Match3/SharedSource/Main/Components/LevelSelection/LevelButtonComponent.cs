using Match3.Services;
using Match3.UI;
using Match3.UI.Navigation;
using System;
using System.Runtime.Serialization;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Toolkit;
using WaveEngine.Framework;

namespace Match3.Components.LevelSelection
{
    [DataContract]
    public class LevelButtonComponent : Component, IDisposable
    {
        [RequiredComponent]
        protected SpriteAtlas spriteAtlas;

        [RequiredComponent]
        protected ButtonComponent buttonComponent;

        [RequiredComponent]
        protected NavigationComponent navComponent;
        
        [RequiredComponent]
        protected TouchGestures touchGestures;

        protected TextComponent textComponent;

        protected SpriteAtlas starLeftSpriteAtlas;

        protected SpriteAtlas starCenterSpriteAtlas;

        protected SpriteAtlas starRightSpriteAtlas;

        [DataMember]
        private int levelIndex;

        [DataMember]
        private int starsCount;

        [DataMember]
        private bool isUnlocked;

        public int LevelIndex
        {
            get
            {
                return levelIndex;
            }

            set
            {
                levelIndex = value;
                this.RefreshVisualState();
            }
        }

        public int StarsCount
        {
            get
            {
                return starsCount;
            }

            set
            {
                starsCount = value;
                this.RefreshVisualState();
            }
        }

        public bool IsUnlocked
        {
            get
            {
                return isUnlocked;
            }

            set
            {
                isUnlocked = value;
                this.RefreshVisualState();
            }
        }

        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.levelIndex = 1;
            this.isUnlocked = false;
        }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            var textEntity = this.Owner.FindChild("Text");
            this.textComponent = textEntity.FindComponent<TextComponent>();

            var starLeftEntity = this.Owner.FindChild("StarLeft");
            this.starLeftSpriteAtlas = starLeftEntity.FindComponent<SpriteAtlas>();
            
            var starCenterEntity = this.Owner.FindChild("StarCenter");
            this.starCenterSpriteAtlas = starCenterEntity.FindComponent<SpriteAtlas>();
            
            var starRightEntity = this.Owner.FindChild("StarRight");
            this.starRightSpriteAtlas = starRightEntity.FindComponent<SpriteAtlas>();

            this.touchGestures.TouchTap -= this.TouchGestures_TouchTap;
            this.touchGestures.TouchTap += this.TouchGestures_TouchTap;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.RefreshVisualState();
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

        private void RefreshVisualState()
        {
            if (this.isUnlocked)
            {
                this.textComponent.Text = this.levelIndex.ToString();
                this.buttonComponent.IsActive = true;
                this.spriteAtlas.TextureName = WaveContent.Assets.GUI.Buttons_spritesheet_TextureName.BgBlueSquareButton;

                // Refresh stars
                this.starLeftSpriteAtlas.Owner.IsVisible = true;
                this.starCenterSpriteAtlas.Owner.IsVisible = true;
                this.starRightSpriteAtlas.Owner.IsVisible = true;


                this.starLeftSpriteAtlas.TextureName = this.starsCount < 1 ? 
                    WaveContent.Assets.GUI.Panels_spritesheet_TextureName.StarSmallGray :
                    WaveContent.Assets.GUI.Panels_spritesheet_TextureName.StarSmallColor;

                this.starCenterSpriteAtlas.TextureName = this.starsCount < 2 ?
                    WaveContent.Assets.GUI.Panels_spritesheet_TextureName.StarSmallGray :
                    WaveContent.Assets.GUI.Panels_spritesheet_TextureName.StarSmallColor;

                this.starRightSpriteAtlas.TextureName = this.starsCount < 3 ?
                    WaveContent.Assets.GUI.Panels_spritesheet_TextureName.StarSmallGray :
                    WaveContent.Assets.GUI.Panels_spritesheet_TextureName.StarSmallColor;
            }
            else
            {
                this.textComponent.Text = string.Empty;
                this.buttonComponent.IsActive = false;
                this.spriteAtlas.TextureName = WaveContent.Assets.GUI.Buttons_spritesheet_TextureName.LockedButton;

                // Refresh stars
                this.starLeftSpriteAtlas.Owner.IsVisible = false;
                this.starCenterSpriteAtlas.Owner.IsVisible = false;
                this.starRightSpriteAtlas.Owner.IsVisible = false;
            }
        }
        
        private void TouchGestures_TouchTap(object sender, GestureEventArgs e)
        {
            if (!this.isUnlocked)
            {
                return;
            }

            CustomServices.GameLogic.SelectLevel(this.LevelIndex);

            this.navComponent.DoNavigation();
        }
    }
}
