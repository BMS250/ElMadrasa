--In Job (or task) sceduller in stored Procedure
Insert into [dbo].[Absences]
select '2025-10-31', '', 1, ID, NEWID(), 1, 1 from [dbo].[Students];

UPDATE Absences
SET AlhanAttendant = 0, CopticAttendant = 0, TacsAttendant = 0 FROM Absences
INNER JOIN Students ON Absences.StudentId = Students.Id
WHERE Students.State != 0
and AbsenceDate = '2025-10-31';