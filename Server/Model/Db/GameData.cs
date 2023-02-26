using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Model.Db
{
    [Table("game")]
    public class GameData
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("history")]
        public string History { get; set; }
    }
}
