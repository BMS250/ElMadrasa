namespace MyProject.Models.DTOs
{
    public class SubjectAbsenceDTO
    {
        public Student Student { get; set; }
        public bool AlhanAbsence { get; set; }
        public bool CopticAbsence { get; set; }
        public bool TacsAbsence { get; set; }
    }
}
