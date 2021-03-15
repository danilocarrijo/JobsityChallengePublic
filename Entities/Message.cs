using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    [Table("Message")]
    public class Message
    {
        [Column("Id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("User")]
        public string User { get; set; }

        [Column("MessageStrin")]
        public string MessageStrin { get; set; }

        [Column("MessageDateMessage")]
        public DateTime MessageDateMessage { get; set; }
    }
}
