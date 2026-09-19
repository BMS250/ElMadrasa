namespace MyProject.Models.DTOs
{
    public class ClassCapacityDTO
    {
        public int ClassNumber { get; set; }
        public int Capacity { get; set; }
        public int NumberOfAttendants { get; set; }
        public int NumberOfAbsents => Capacity - NumberOfAttendants;
    }
}
