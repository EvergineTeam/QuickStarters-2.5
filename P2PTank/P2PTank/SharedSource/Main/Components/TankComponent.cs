using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace P2PTank.Components
{
    public class TankComponent : Component
    {
        private float initialLive = 100;
        private float initialSpeed = 100;
        private float initialRotationSpeed = 2;

        public float CurrentLive { get; private set; }

        public float CurrentSpeed { get; private set; }

        public float CurrentRotationSpeed { get; private set; }

        public TankComponent()
        {
            this.CurrentLive = this.initialLive;
            this.CurrentSpeed = this.initialSpeed;
            this.CurrentRotationSpeed = this.initialRotationSpeed;
        }

    }
}
