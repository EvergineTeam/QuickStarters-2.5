using System;
using System.Collections.Generic;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
using WaveEngine.Networking;
using WaveEngine.Networking.Messages;

namespace MultiplayerTopDownTank.Scenes
{
    public class LobbyScene : Scene
    {
        private const int MinIndex = 1;
        private const int MaxIndex = 4;
        private readonly List<int> assignedPlayerIndex;

        private TextBlock messageTextBlock;

        private readonly NetworkService networkService;

        public LobbyScene()
        {
            this.networkService = WaveServices.GetService<NetworkService>();
            this.networkService.MessageReceivedFromHost += this.ClientMessageReceived;
            this.networkService.MessageReceivedFromClient += this.HostMessageReceived;

            assignedPlayerIndex = new List<int>();
        }

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.LobbyScene);

            var camera2D = new FixedCamera2D("Camera2D")
            {
                BackgroundColor = Color.CornflowerBlue
            };

            this.EntityManager.Add(camera2D);

            this.messageTextBlock = new TextBlock()
            {
                Width = 600,
                Height = 50,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(10, 0, 10, 10)
            };
            this.EntityManager.Add(this.messageTextBlock);

            this.SendHelloToServer();
        }

        private void SendHelloToServer()
        {
            messageTextBlock.Text = "Waiting player assignment...";

            this.SelectPlayer(WaveServices.Random.Next(MinIndex, MaxIndex));
        }

        private void SelectPlayer(int playerIndex)
        {
            var message = this.networkService.CreateClientMessage();
            message.Write(this.networkService.ClientIdentifier);
            message.Write(playerIndex);

            this.networkService.SendToServer(message, DeliveryMethod.ReliableUnordered);
        }

        private int AssignPlayerIndex(int playerIndex)
        {
            lock (this)
            {
                if (this.assignedPlayerIndex.Contains(playerIndex))
                {
                    playerIndex = this.GetNextPlayerIndex();
                }

                this.assignedPlayerIndex.Add(playerIndex);
            }

            return playerIndex;
        }

        private int GetNextPlayerIndex()
        {
            for (int i = MinIndex; i <= MaxIndex; i++)
            {
                if (!this.assignedPlayerIndex.Contains(i))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Handles the messages received from the clients. Only when this player is the host.
        /// </summary>
        private void HostMessageReceived(object sender, NetworkEndpoint networkEndpoint, IncomingMessage receivedMessage)
        {
            var playerIdentifier = receivedMessage.ReadString();
            var playerIndex = receivedMessage.ReadInt32();

            var resultPlayerIndex = this.AssignPlayerIndex(playerIndex);

            var responseMessage = this.networkService.CreateServerMessage();
            responseMessage.Write(playerIdentifier);
            responseMessage.Write(resultPlayerIndex);

            this.networkService.SendToClients(responseMessage, DeliveryMethod.ReliableUnordered);
        }

        /// <summary>
        /// Handles the messages received from the host.
        /// </summary>
        private void ClientMessageReceived(object sender, NetworkEndpoint networkEndpoint, IncomingMessage receivedMessage)
        {
            var playerIdentifier = receivedMessage.ReadString();
            var playerIndex = receivedMessage.ReadInt32();

            if (this.networkService.ClientIdentifier == playerIdentifier)
            {
                this.HandlePlayerSelectionResponse(playerIndex);
            }
        }

        /// <summary>
        /// Handles the player selection response.
        /// </summary>
        /// <param name="playerIndex">Index of the player.</param>
        private void HandlePlayerSelectionResponse(int playerIndex)
        {
            if (playerIndex < 0)
            {
                this.ServerCompleted();
            }
            else
            {
                this.PlayerSelected(playerIndex);
            }
        }

        protected override void End()
        {
            base.End();

            this.networkService.MessageReceivedFromHost -= this.ClientMessageReceived;
            this.networkService.MessageReceivedFromClient -= this.HostMessageReceived;
        }

        private void PlayerSelected(int playerIndex)
        {
            // Wait 3 seconds and start game.
            int remainingSeconds = 3;
            var timerName = "PlayerSelectedTimer";
            this.UpdateRemainingSeconds(remainingSeconds);
            WaveServices.TimerFactory.CreateTimer(timerName, TimeSpan.FromSeconds(1), () =>
            {
                remainingSeconds--;
                if (remainingSeconds == 0)
                {
                    WaveServices.TimerFactory.RemoveTimer(timerName);

                    // Navigate to GameScene and created player with selected sprite.
                    WaveServices.ScreenContextManager.Push(new ScreenContext(new GameScene(playerIndex)));
                }

                this.UpdateRemainingSeconds(remainingSeconds);
            }, true, this);
        }

        private void UpdateRemainingSeconds(int remainingSeconds)
        {
            messageTextBlock.Text = string.Format("Player assigned. Starting in {0} second(s)", remainingSeconds);
        }

        private void ServerCompleted()
        {
            // Sprite selection not allowed, server completed, disconnect, and navigate back.
            this.networkService.Disconnect();
            WaveServices.TimerFactory.CreateTimer(TimeSpan.FromSeconds(1), () =>
            {
                WaveServices.ScreenContextManager.Pop(false);
            }, false, this);
        }
    }
}