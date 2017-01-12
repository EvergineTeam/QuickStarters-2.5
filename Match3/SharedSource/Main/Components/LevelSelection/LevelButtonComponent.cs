using Match3.UI;
using System;
using System.Runtime.Serialization;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Toolkit;
using WaveEngine.Framework;

namespace Match3.Components.LevelSelection
{
    [DataContract]
    public class LevelButtonComponent : Component
    {
        [RequiredComponent]
        protected SpriteAtlas spriteAtlas;

        [RequiredComponent]
        protected ButtonComponent buttonComponent;

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
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.RefreshVisualState();
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
    }
}
