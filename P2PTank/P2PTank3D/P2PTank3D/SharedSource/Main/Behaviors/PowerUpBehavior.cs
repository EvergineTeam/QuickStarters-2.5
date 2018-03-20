using P2PTank.Managers;
using P2PTank.Scenes;
using System;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace P2PTank.Behaviors
{
    [DataContract]
    public class PowerUpBehavior : Behavior
    {
        private const int LifeTime = 10;

        private PowerUpManager powerUpManager;
        private TimeSpan currentLifeTime;
        private Material bulletMaterial = null;
        private Material repairMaterial = null;

        [IgnoreDataMember]
        public PowerUpType PowerUpType { get; set; }

        [DataMember]
        [RenderPropertyAsAsset(AssetType.Material)]
        public string BulletMaterialPath { get; set; }

        [DataMember]
        [RenderPropertyAsAsset(AssetType.Material)]
        public string RepairMaterialPath { get; set; }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.powerUpManager = this.Owner.Scene.EntityManager.FindComponentFromEntityPath<PowerUpManager>(GameConstants.ManagerEntityPath);

            this.currentLifeTime = TimeSpan.FromSeconds(LifeTime);

            var model3D = this.Owner.FindComponentsInChildren<MaterialComponent>().FirstOrDefault();

            switch (this.PowerUpType)
            {
                case PowerUpType.Bullet:
                    model3D.MaterialPath = this.BulletMaterialPath;
                    break;
                case PowerUpType.Repair:
                    model3D.MaterialPath = this.RepairMaterialPath;
                    break;
            }

            model3D.UseCopy = true;
        }

        protected override void Update(TimeSpan gameTime)
        {
            this.currentLifeTime = this.currentLifeTime - gameTime;

            if (this.currentLifeTime <= TimeSpan.Zero)
            {
                this.powerUpManager.SendDestroyPowerUpMessage(this.Owner.Name);
            }
        }
    }
}