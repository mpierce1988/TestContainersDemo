RESTORE DATABASE AdventureWorks
FROM DISK = '/var/opt/mssql/backup/AdventureWorksLT2019.bak'
WITH MOVE 'AdventureWorksLT2019_Data' TO '/var/opt/mssql/data/AdventureWorksLT2019_Data.mdf',
     MOVE 'AdventureWorksLT2019_Log' TO '/var/opt/mssql/data/AdventureWorksLT2019_Log.ldf',
     RECOVERY, STATS=5;