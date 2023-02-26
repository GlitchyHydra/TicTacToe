namespace Server
{
    public class Player
    {
        public string Id { get; private set; }
        public string Username { get; private set; }
        public string GameId { get; set; }
        public string Shape { get; set; }
        public bool IsReadyToPlay { get; set; }

        public Player(string username, string id)
        {
            Id = id;
            Username = username;
            IsReadyToPlay = false;
        }

        public override bool Equals(object obj)
        {
            Player other = obj as Player;

            if (other == null)
            {
                return false;
            }

            return Id.Equals(other.Id) && Username.Equals(other.Username);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() * Username.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("(Id={0}, Name={1}, GameId={2}, Piece={3})",
                Id, Username, GameId, Shape);
        }
    }
}
