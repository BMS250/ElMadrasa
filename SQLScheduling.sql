EXEC msdb.dbo.sp_add_job
    @job_name = 'RunAbsencesJobEveryFridayAt3PM',
    @enabled = 1;


EXEC msdb.dbo.sp_add_jobstep
@job_name = 'RunAbsencesJobEveryFridayAt3PM',
@step_name = 'InsertAndUpdateAbsences',
@subsystem = 'TSQL',
@command = '
    INSERT INTO [dbo].[Absences] 
    SELECT ID, GETDATE(), '', 1 
    FROM [dbo].[Students];

    UPDATE Absences
    SET Attendant = 0
    FROM Absences 
    INNER JOIN Students 
    ON Absences.StudentId = Students.Id
    WHERE Students.Notes IN (N''«Ê‰·«Ì‰'', N''„ƒÃ·'', N''·«€Ï'') 
    AND AbsenceDate = GETDATE();',
@database_name = 'db9491';


EXEC msdb.dbo.sp_add_schedule
    @schedule_name = 'EveryFridayAt3PM',
    @enabled = 1,
    @freq_type = 4,  -- Weekly
    @freq_interval = 64,  -- Every Friday
    @active_start_time = 150000,  -- 3 PM
    @active_start_date = 20241227,  -- Start date (example)
    @timezone = 'Egypt Standard Time';  -- Timezone


EXEC msdb.dbo.sp_attach_schedule
@job_name = 'RunAbsencesJobEveryFridayAt3PM',
@schedule_name = 'EveryFridayAt3PM';
