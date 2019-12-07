using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureNotepad.Models
{
    public class Note
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string CypherText { get; set; }
    }
}
