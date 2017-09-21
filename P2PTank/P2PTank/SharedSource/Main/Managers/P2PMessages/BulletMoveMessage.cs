namespace P2PTank.Entities.P2PMessages
{
    public class BulletMoveMessage
    {
        public string BulletId { get; set; }

        public string PlayerId { get; set; }

        public float X { get; set; }

        public float Y { get; set; }
    }
}
