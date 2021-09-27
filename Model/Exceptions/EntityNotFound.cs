using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Exceptions {
    public class EntityNotFound : Exception {
        public long Id { get; set; }
        public EntityNotFound (string message , long id) : base(string.Format(message,id))
        {
            Id = id;
        }
    }
}
