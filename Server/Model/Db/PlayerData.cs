using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Model.Db
{
    public enum PlayerRole
    {
        [Description("player")]
        Player,
        [Description("spectator")]
        Spectator
    }

    [Table("player")]
    public class PlayerData
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("role"), EnumDataType(typeof(PlayerRole))]
        public PlayerRole Role { get; set; }
        [Column("token")] 
        public string? Token { get; set; }

    }
}
