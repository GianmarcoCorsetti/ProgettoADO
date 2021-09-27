using System;

namespace Model.Entities {
    public class Classroom {
        public long Id { get; set; }
        public string Name { get; set; }
        public int MaxCapacity { get; set; }
        public bool IsPhysical { get; set; }
        public bool IsComputerized { get; set; }
        public bool? HasProjector { get; set; }

        public Classroom( long newId, string newName, int newMaxCapacity, bool isPhysical, bool isComputerized, bool? hasProjector )
        {
            Id = newId;
            Name = newName;
            MaxCapacity = newMaxCapacity;
            IsPhysical = isPhysical;
            IsComputerized = isComputerized;
            HasProjector = hasProjector;
        }
        public Classroom()
        {

        }
    }
}
