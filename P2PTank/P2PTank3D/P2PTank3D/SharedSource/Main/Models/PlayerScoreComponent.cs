using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Toolkit;
using WaveEngine.Framework;

namespace P2PTank3D.Models
{
    [DataContract]
    public class PlayerScoreComponent : Component
    {
        private Color color;
        private Sprite tankBody;
        private TextComponent tbKills;
        private TextComponent tbDeads;

        [IgnoreDataMember]
        public string PlayerID { get; set; }

        [IgnoreDataMember]
        public Color Color
        {
            get { return this.color; }
            set
            {
                this.color = value;

                UpdateValues();
            }
        }

        [IgnoreDataMember]
        public int Kills { get; internal set; }

        [IgnoreDataMember]
        public int Deads { get; internal set; }

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.tankBody = this.Owner?.FindChild("tankBody")?.FindComponent<Sprite>();
            this.tbKills = this.Owner?.FindChild("kills")?.FindComponent<TextComponent>();
            this.tbDeads = this.Owner?.FindChild("deads")?.FindComponent<TextComponent>();
        }

        public PlayerScoreComponent()
        {
            this.OnComponentInitialized -= this.PlayerScoreComponentOnComponentInitialized;
            this.OnComponentInitialized += this.PlayerScoreComponentOnComponentInitialized;
        }

        private void PlayerScoreComponentOnComponentInitialized(object sender, EventArgs e)
        {
            this.UpdateValues();
        }

        private void UpdateValues()
        {
            if (this.tankBody != null)
                this.tankBody.TintColor = this.Color;

            if (this.tbKills != null)
            this.tbKills.Text = this.Kills.ToString();

            if (this.tbDeads != null)
            this.tbDeads.Text = this.Deads.ToString();
        }

        public void Loose()
        {
            this.Deads++;
            this.UpdateValues();
        }

        public void Victory()
        {
            this.Kills++;
            this.UpdateValues();
        }

        public void Reset()
        {
            this.Kills = 0;
            this.Deads = 0;
            this.UpdateValues();
        }
    }
}
