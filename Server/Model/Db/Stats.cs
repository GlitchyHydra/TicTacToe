using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Model.Db
{
    [Table("stats")]
    public class Stats
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("res")]
        public string Result { get; set; }
        [Column("game_id")]
        public int GameId { get; set; }
        [Column("player_id")]
        public int PlayerId { get; set; }
    }
}
