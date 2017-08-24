using System;
using System.Collections.Generic;
using System.Text;

namespace SuperSquid.Services
{
    public class GameplayService
    {
        private int currentScore;

        public int CurrentScore
        {
            get
            {
                return this.currentScore;
            }

            set
            {
                this.currentScore = value;
            }
        }

        public void Start()
        {
        }

        public void Pause()
        {
        }

        public void Reset()
        {
        }
    }
}
