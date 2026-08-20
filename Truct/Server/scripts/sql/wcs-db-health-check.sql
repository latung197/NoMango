/*
  WCS — SQL Server Health Check
  =============================
  Chạy trên SSMS / Azure Data Studio, kết nối tới instance SQL Server của WCS.

  Cách dùng:
    1. Chạy toàn bộ script (F5) — mỗi section in ra một result set riêng.
    2. Chạy lúc hệ thống đang chậy để thấy blocking / wait stats thực tế.
    3. So sánh kết quả lúc bình thường vs lúc chậm.

  Lưu ý:
    - Một số DMV (sys.dm_exec_query_stats) reset khi SQL Server restart.
    - Query Store cần bật trước — section 12 sẽ báo nếu chưa bật.
    - Thay @DatabaseName nếu tên DB khác môi trường production.
*/

SET NOCOUNT ON;

DECLARE @DatabaseName sysname = N'WcsDb';
DECLARE @CapDatabaseName sysname = N'CapWcsDb';
DECLARE @ServerStartTime datetime = (
    SELECT sqlserver_start_time FROM sys.dm_os_sys_info
);

PRINT '================================================================';
PRINT ' WCS DB Health Check';
PRINT ' Thời gian chạy : ' + CONVERT(varchar(30), SYSDATETIME(), 120);
PRINT ' SQL khởi động  : ' + CONVERT(varchar(30), @ServerStartTime, 120);
PRINT ' Uptime (giờ)   : ' + CONVERT(varchar(30), CAST(DATEDIFF(MINUTE, @ServerStartTime, GETDATE()) / 60.0 AS decimal(10,1)));
PRINT '================================================================';

-- ----------------------------------------------------------------
-- 1. Tổng quan instance
-- ----------------------------------------------------------------
PRINT '';
PRINT '--- 1. Tổng quan SQL Server instance ---';

SELECT
    cpu_count                          AS [Logical CPU],
    (physical_memory_kb / 1024)         AS [RAM MB],
    committed_target_kb / 1024         AS [Target Memory MB],
    committed_kb / 1024                AS [Committed Memory MB],
    max_workers_count                  AS [Max Workers],
    scheduler_count                    AS [Schedulers]
FROM sys.dm_os_sys_info;

SELECT
    cntr_value AS [User Connections]
FROM sys.dm_os_performance_counters
WHERE counter_name = N'User Connections'
  AND object_name LIKE N'%General Statistics%';

SELECT
    cntr_value AS [Batch Requests/sec]
FROM sys.dm_os_performance_counters
WHERE counter_name = N'Batch Requests/sec'
  AND object_name LIKE N'%SQL Statistics%';

-- ----------------------------------------------------------------
-- 2. Buffer cache & memory pressure
-- ----------------------------------------------------------------
PRINT '';
PRINT '--- 2. Buffer cache hit ratio (nên > 95%) ---';

SELECT
    CAST(a.cntr_value * 100.0 / NULLIF(b.cntr_value, 0) AS decimal(5,2)) AS [Buffer Cache Hit Ratio %]
FROM sys.dm_os_performance_counters a
JOIN sys.dm_os_performance_counters b
    ON a.object_name = b.object_name
WHERE a.counter_name = N'Buffer cache hit ratio'
  AND b.counter_name = N'Buffer cache hit ratio base'
  AND a.object_name LIKE N'%Buffer Manager%';

SELECT
    (cntr_value * 8.0 / 1024 / 1024) AS [Buffer Pool GB]
FROM sys.dm_os_performance_counters
WHERE counter_name = N'Database pages'
  AND object_name LIKE N'%Buffer Manager%';

SELECT TOP 10
    DB_NAME(database_id)               AS [Database],
    COUNT(*) * 8 / 1024                AS [Cached MB]
FROM sys.dm_os_buffer_descriptors
WHERE database_id > 4
GROUP BY database_id
ORDER BY [Cached MB] DESC;

-- ----------------------------------------------------------------
-- 3. Disk I/O latency (nguyên nhân phổ biến khi CPU thấp mà vẫn chậm)
-- ----------------------------------------------------------------
PRINT '';
PRINT '--- 3. Disk I/O latency theo file DB (avg_read_ms > 20 = cần xem xét) ---';

SELECT
    DB_NAME(vfs.database_id)                                    AS [Database],
    mf.type_desc                                                AS [File Type],
    mf.physical_name                                            AS [Physical Path],
    vfs.num_of_reads                                            AS [Reads],
    vfs.io_stall_read_ms                                        AS [Read Stall ms],
    CASE WHEN vfs.num_of_reads = 0 THEN 0
         ELSE vfs.io_stall_read_ms / vfs.num_of_reads END       AS [Avg Read ms],
    vfs.num_of_writes                                           AS [Writes],
    vfs.io_stall_write_ms                                       AS [Write Stall ms],
    CASE WHEN vfs.num_of_writes = 0 THEN 0
         ELSE vfs.io_stall_write_ms / vfs.num_of_writes END     AS [Avg Write ms]
FROM sys.dm_io_virtual_file_stats(NULL, NULL) vfs
JOIN sys.master_files mf
    ON vfs.database_id = mf.database_id
   AND vfs.file_id = mf.file_id
WHERE DB_NAME(vfs.database_id) IN (@DatabaseName, @CapDatabaseName)
ORDER BY [Avg Read ms] DESC, [Avg Write ms] DESC;

-- ----------------------------------------------------------------
-- 4. Session đang chạy & blocking
-- ----------------------------------------------------------------
PRINT '';
PRINT '--- 4. Active requests (wait_time cao = đang chờ I/O / lock / network) ---';

SELECT
    r.session_id,
    r.status,
    r.command,
    r.wait_type,
    r.wait_time / 1000.0                AS [Wait sec],
    r.blocking_session_id,
    DB_NAME(r.database_id)              AS [Database],
    r.cpu_time,
    r.logical_reads,
    r.reads,
    r.writes,
    SUBSTRING(
        t.text,
        (r.statement_start_offset / 2) + 1,
        (
            (CASE r.statement_end_offset
                WHEN -1 THEN DATALENGTH(t.text)
                ELSE r.statement_end_offset
             END - r.statement_start_offset) / 2
        ) + 1
    )                                   AS [Query Text]
FROM sys.dm_exec_requests r
CROSS APPLY sys.dm_exec_sql_text(r.sql_handle) t
WHERE r.session_id <> @@SPID
  AND r.session_id > 50
ORDER BY r.wait_time DESC;

PRINT '';
PRINT '--- 4b. Blocking chain ---';

SELECT
    blocked.session_id                  AS [Blocked Session],
    blocking.session_id                 AS [Blocking Session],
    blocked.wait_type,
    blocked.wait_time / 1000.0          AS [Blocked Wait sec],
    DB_NAME(blocked.database_id)        AS [Database]
FROM sys.dm_exec_requests blocked
JOIN sys.dm_exec_requests blocking
    ON blocked.blocking_session_id = blocking.session_id
WHERE blocked.blocking_session_id <> 0;

-- ----------------------------------------------------------------
-- 5. Top wait types (tích lũy từ lúc restart)
-- ----------------------------------------------------------------
PRINT '';
PRINT '--- 5. Top wait types (PAGEIOLATCH=I/O, LCK_M=lock, ASYNC_NETWORK_IO=client chậm) ---';

SELECT TOP 15
    wait_type,
    waiting_tasks_count,
    wait_time_ms / 1000.0               AS [Wait sec],
    CAST(100.0 * wait_time_ms / SUM(wait_time_ms) OVER () AS decimal(5,2)) AS [Pct],
    wait_time_ms / NULLIF(waiting_tasks_count, 0) AS [Avg Wait ms]
FROM sys.dm_os_wait_stats
WHERE wait_type NOT IN (
    -- waits thông thường, ít ý nghĩa chẩn đoán
    N'BROKER_EVENTHANDLER', N'BROKER_RECEIVE_WAITFOR', N'BROKER_TASK_STOP',
    N'BROKER_TO_FLUSH', N'BROKER_TRANSMITTER', N'CHECKPOINT_QUEUE',
    N'CHUNKED_OPERATION_CREATE_BUFFER', N'CLR_AUTO_EVENT', N'CLR_MANUAL_EVENT',
    N'CLR_SEMAPHORE', N'DBMIRROR_DBM_EVENT', N'DBMIRROR_EVENTS_QUEUE',
    N'DBMIRROR_WORKER_QUEUE', N'DBMIRRORING_CMD', N'DIRTY_PAGE_POLL',
    N'DISPATCHER_QUEUE_SEMAPHORE', N'EXECSYNC', N'FAILED_INITIATE_OPER',
    N'FT_IFTS_SCHEDULER_IDLE_WAIT', N'FT_IFTSHC_MUTEX', N'HADR_CLUSAPI_CALL',
    N'HADR_FILESTREAM_IOMGR_IOCOMPLETION', N'HADR_LOGCAPTURE_WAIT',
    N'HADR_NOTIFICATION_DEQUEUE', N'HADR_TIMER_TASK', N'HADR_WORK_QUEUE',
    N'KSOURCE_WAKEUP', N'LAZYWRITER_SLEEP', N'LOGMGR_QUEUE',
    N'MEMORY_ALLOCATION_EXT', N'ONDEMAND_TASK_QUEUE', N'PARALLEL_REDO_DRAIN_WORKER',
    N'PARALLEL_REDO_LOG_CACHE', N'PARALLEL_REDO_TRAN_LIST', N'PARALLEL_REDO_WORKER_SYNC',
    N'PARALLEL_REDO_WORKER_WAIT_WORK', N'PREEMPTIVE_OS_FLUSHFILEBUFFERS',
    N'PREEMPTIVE_XE_GETTARGETSTATE', N'PWAIT_EXTensibilityLoad',
    N'PWAIT_ALL_COMPONENTS_INITIALIZED', N'PWAIT_DIRECTLOGCONSUMER_GETNEXT',
    N'QDS_PERSIST_TASK_MAIN_LOOP_SLEEP', N'QDS_ASYNC_QUEUE',
    N'QDS_CLEANUP_STALE_QUERIES_TASK_MAIN_LOOP_SLEEP',
    N'REDO_THREAD_PENDING_WORK', N'REQUEST_FOR_DEADLOCK_SEARCH',
    N'RESOURCE_QUEUE', N'SERVER_IDLE_CHECK', N'SLEEP_BPOOL_FLUSH',
    N'SLEEP_DBSTARTUP', N'SLEEP_DCOMSTARTUP', N'SLEEP_MASTERDBREADY',
    N'SLEEP_MASTERMDREADY', N'SLEEP_MASTERUPGRADED', N'SLEEP_MSDBSTARTUP',
    N'SLEEP_SYSTEMTASK', N'SLEEP_TASK', N'SLEEP_TEMPDBSTARTUP',
    N'SNI_HTTP_ACCEPT', N'SOS_WORK_DISPATCHER', N'SP_SERVER_DIAGNOSTICS_SLEEP',
    N'SQLTRACE_BUFFER_FLUSH', N'SQLTRACE_INCREMENTAL_FLUSH_SLEEP',
    N'SQLTRACE_WAIT_ENTRIES', N'WAIT_FOR_RESULTS', N'WAITFOR',
    N'WAITFOR_TASKSHUTDOWN', N'WAIT_XTP_HOST_WAIT',
    N'WAIT_XTP_OFFLINE_CKPT_NEW_LOG', N'WAIT_XTP_CKPT_CLOSE',
    N'XE_BUFFERMGR_ALLPROCESSED_EVENT', N'XE_DISPATCHER_JOIN',
    N'XE_DISPATCHER_WAIT', N'XE_LIVE_TARGET_TVF', N'XE_TIMER_EVENT'
)
ORDER BY wait_time_ms DESC;

-- ----------------------------------------------------------------
-- 6. Top query theo tổng thời gian
-- ----------------------------------------------------------------
PRINT '';
PRINT '--- 6. Top 20 query theo total elapsed time (từ lúc restart) ---';

SELECT TOP 20
    qs.execution_count,
    qs.total_elapsed_time / 1000                        AS [Total ms],
    qs.total_elapsed_time / qs.execution_count / 1000   AS [Avg ms],
    qs.total_logical_reads                            AS [Total Logical Reads],
    qs.total_logical_reads / qs.execution_count       AS [Avg Logical Reads],
    qs.total_worker_time / 1000                       AS [Total CPU ms],
    qs.last_execution_time,
    SUBSTRING(
        st.text,
        (qs.statement_start_offset / 2) + 1,
        (
            (CASE qs.statement_end_offset
                WHEN -1 THEN DATALENGTH(st.text)
                ELSE qs.statement_end_offset
             END - qs.statement_start_offset) / 2
        ) + 1
    )                                                 AS [Query Text]
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) st
WHERE st.dbid = DB_ID(@DatabaseName)
   OR st.text LIKE N'%' + @DatabaseName + N'%'
   OR st.text LIKE N'%TransferRequest%'
   OR st.text LIKE N'%FlowTask%'
   OR st.text LIKE N'%Station%'
   OR st.text LIKE N'%Amr%'
ORDER BY qs.total_elapsed_time DESC;

-- ----------------------------------------------------------------
-- 7. Top query theo số lần gọi (chatty queries)
-- ----------------------------------------------------------------
PRINT '';
PRINT '--- 7. Top 20 query theo execution count (nhiều round-trip nhỏ) ---';

SELECT TOP 20
    qs.execution_count,
    qs.total_elapsed_time / qs.execution_count / 1000   AS [Avg ms],
    qs.total_logical_reads / qs.execution_count         AS [Avg Logical Reads],
    qs.last_execution_time,
    SUBSTRING(
        st.text,
        (qs.statement_start_offset / 2) + 1,
        (
            (CASE qs.statement_end_offset
                WHEN -1 THEN DATALENGTH(st.text)
                ELSE qs.statement_end_offset
             END - qs.statement_start_offset) / 2
        ) + 1
    )                                                 AS [Query Text]
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) st
WHERE qs.execution_count > 100
ORDER BY qs.execution_count DESC;

-- ----------------------------------------------------------------
-- 8. Kích thước DB & bảng WcsDb
-- ----------------------------------------------------------------
PRINT '';
PRINT '--- 8. Kích thước database ---';

SELECT
    name                                AS [Database],
    state_desc                          AS [State],
    recovery_model_desc                 AS [Recovery Model],
    (SELECT SUM(size) * 8 / 1024 FROM sys.master_files mf WHERE mf.database_id = d.database_id) AS [Size MB]
FROM sys.databases d
WHERE name IN (@DatabaseName, @CapDatabaseName);

PRINT '';
PRINT '--- 8b. Row count & size các bảng chính WcsDb ---';

IF DB_ID(@DatabaseName) IS NOT NULL
BEGIN
    EXEC(N'
    USE [' + @DatabaseName + N'];
    SELECT
        t.name                              AS [Table],
        SUM(p.rows)                         AS [Rows],
        SUM(a.total_pages) * 8 / 1024       AS [Total MB],
        SUM(a.used_pages) * 8 / 1024        AS [Used MB]
    FROM sys.tables t
    JOIN sys.partitions p ON t.object_id = p.object_id AND p.index_id IN (0, 1)
    JOIN sys.allocation_units a ON p.partition_id = a.container_id
    WHERE t.is_ms_shipped = 0
      AND t.name IN (
          N''FlowTasks'', N''FlowTaskWaitingSets'', N''FlowTaskHistory'',
          N''TransferRequests'', N''Stations'', N''StationTags'',
          N''Amrs'', N''Cassettes'', N''CraneTaskDispatches'', N''Settings''
      )
    GROUP BY t.name
    ORDER BY [Rows] DESC;
    ');
END
ELSE
    PRINT 'Database ' + @DatabaseName + ' không tồn tại trên instance này.';

-- ----------------------------------------------------------------
-- 9. TransferRequests & FlowTasks — trạng thái hiện tại
-- ----------------------------------------------------------------
PRINT '';
PRINT '--- 9. Phân bố status TransferRequests & FlowTasks ---';

IF DB_ID(@DatabaseName) IS NOT NULL
BEGIN
    EXEC(N'
    USE [' + @DatabaseName + N'];
    SELECT Status, COUNT(*) AS [Count]
    FROM TransferRequests
    GROUP BY Status
    ORDER BY [Count] DESC;

    SELECT Status, CurrentStep, COUNT(*) AS [Count]
    FROM FlowTasks
    WHERE Status <> 3  -- Completed
    GROUP BY Status, CurrentStep
    ORDER BY [Count] DESC;
    ');
END

-- ----------------------------------------------------------------
-- 10. Missing index (gợi ý từ SQL Server)
-- ----------------------------------------------------------------
PRINT '';
PRINT '--- 10. Missing index suggestions cho WcsDb ---';

IF DB_ID(@DatabaseName) IS NOT NULL
BEGIN
    SELECT
        migs.avg_user_impact                              AS [Avg Impact %],
        migs.user_seeks + migs.user_scans                   AS [Seeks+Scans],
        OBJECT_NAME(mid.object_id, mid.database_id)         AS [Table],
        mid.equality_columns,
        mid.inequality_columns,
        mid.included_columns,
        migs.avg_total_user_cost * (migs.user_seeks + migs.user_scans) AS [Improvement Score],
        N'CREATE NONCLUSTERED INDEX IX_'
            + OBJECT_NAME(mid.object_id, mid.database_id)
            + N'_' + REPLACE(REPLACE(REPLACE(ISNULL(mid.equality_columns, N''), N', ', N'_'), N'[', N''), N']', N'')
            + N' ON ' + mid.statement
            + N' (' + ISNULL(mid.equality_columns, N'')
            + CASE WHEN mid.inequality_columns IS NOT NULL
                   THEN N', ' + mid.inequality_columns ELSE N'' END
            + N')'
            + CASE WHEN mid.included_columns IS NOT NULL
                   THEN N' INCLUDE (' + mid.included_columns + N')' ELSE N'' END
            AS [Suggested DDL]
    FROM sys.dm_db_missing_index_details mid
    JOIN sys.dm_db_missing_index_groups mig ON mid.index_handle = mig.index_handle
    JOIN sys.dm_db_missing_index_group_stats migs ON mig.index_group_handle = migs.group_handle
    WHERE mid.database_id = DB_ID(@DatabaseName)
    ORDER BY [Improvement Score] DESC;
END

-- ----------------------------------------------------------------
-- 11. Index không được dùng (cân nhắc xóa nếu write-heavy)
-- ----------------------------------------------------------------
PRINT '';
PRINT '--- 11. Index ít/không được seek/scan (user_seeks + user_scans = 0) ---';

IF DB_ID(@DatabaseName) IS NOT NULL
BEGIN
    EXEC(N'
    USE [' + @DatabaseName + N'];
    SELECT
        OBJECT_NAME(i.object_id)            AS [Table],
        i.name                              AS [Index],
        i.type_desc                         AS [Type],
        us.user_seeks,
        us.user_scans,
        us.user_lookups,
        us.user_updates
    FROM sys.indexes i
    LEFT JOIN sys.dm_db_index_usage_stats us
        ON i.object_id = us.object_id
       AND i.index_id = us.index_id
       AND us.database_id = DB_ID()
    WHERE i.object_id > 100
      AND i.type_desc <> N''HEAP''
      AND i.is_primary_key = 0
      AND i.is_unique_constraint = 0
      AND ISNULL(us.user_seeks, 0) + ISNULL(us.user_scans, 0) = 0
    ORDER BY us.user_updates DESC;
    ');
END

-- ----------------------------------------------------------------
-- 12. Query Store status
-- ----------------------------------------------------------------
PRINT '';
PRINT '--- 12. Query Store ---';

SELECT
    d.name                              AS [Database],
    d.is_query_store_on                 AS [Query Store On]
FROM sys.databases d
WHERE d.name IN (@DatabaseName, @CapDatabaseName);

IF EXISTS (
    SELECT 1 FROM sys.databases
    WHERE name = @DatabaseName AND is_query_store_on = 1
)
BEGIN
    EXEC(N'
    USE [' + @DatabaseName + N'];
    SELECT
        desired_state_desc              AS [Desired State],
        actual_state_desc               AS [Actual State],
        current_storage_size_mb         AS [Current Storage MB],
        max_storage_size_mb             AS [Max Storage MB]
    FROM sys.database_query_store_options;

    SELECT TOP 10
        qsq.query_id,
        qsrs.count_executions,
        qsrs.avg_duration / 1000.0        AS [Avg Duration ms],
        qsrs.avg_logical_io_reads           AS [Avg Logical Reads],
        LEFT(qst.query_sql_text, 300)       AS [Query Text]
    FROM sys.query_store_runtime_stats qsrs
    JOIN sys.query_store_plan qsp ON qsrs.plan_id = qsp.plan_id
    JOIN sys.query_store_query qsq ON qsp.query_id = qsq.query_id
    JOIN sys.query_store_query_text qst ON qsq.query_text_id = qst.query_text_id
    ORDER BY qsrs.avg_duration * qsrs.count_executions DESC;
    ');
END
ELSE
BEGIN
    PRINT 'Query Store chưa bật. Khuyến nghị bật để theo dõi lâu dài:';
    PRINT '  ALTER DATABASE ' + @DatabaseName + ' SET QUERY_STORE = ON;';
END

-- ----------------------------------------------------------------
-- 13. CAP message queue (CapWcsDb) — outbox/inbox backlog
-- ----------------------------------------------------------------
PRINT '';
PRINT '--- 13. CAP queue backlog (CapWcsDb) ---';

IF DB_ID(@CapDatabaseName) IS NOT NULL
BEGIN
    IF EXISTS (
        SELECT 1 FROM sys.databases d
        JOIN sys.tables t ON 1=1
        WHERE d.name = @CapDatabaseName
          AND t.name = N'Published' AND t.schema_id = SCHEMA_ID(N'cap')
    )
    BEGIN
        EXEC(N'
        USE [' + @CapDatabaseName + N'];
        SELECT N''Published (chưa gửi)'' AS [Queue], COUNT(*) AS [Count]
        FROM cap.Published WHERE StatusName = N''Scheduled''
        UNION ALL
        SELECT N''Received (chưa xử lý)'', COUNT(*)
        FROM cap.Received WHERE StatusName = N''Scheduled'';
        ');
    END
    ELSE
        PRINT 'Không tìm thấy bảng cap.Published — có thể CAP chưa khởi tạo schema.';
END

-- ----------------------------------------------------------------
-- 14. Connection theo app / login
-- ----------------------------------------------------------------
PRINT '';
PRINT '--- 14. Connections theo program & login ---';

SELECT
    COALESCE(DB_NAME(r.database_id), N'(idle/unknown)') AS [Database],
    s.program_name,
    s.login_name,
    s.host_name,
    COUNT(*)                            AS [Sessions],
    SUM(CASE WHEN r.session_id IS NOT NULL THEN 1 ELSE 0 END) AS [Active]
FROM sys.dm_exec_sessions s
LEFT JOIN sys.dm_exec_requests r ON s.session_id = r.session_id
WHERE s.is_user_process = 1
GROUP BY COALESCE(DB_NAME(r.database_id), N'(idle/unknown)'), s.program_name, s.login_name, s.host_name
ORDER BY [Sessions] DESC;

-- ----------------------------------------------------------------
-- 15. Deadlock history (nếu có extended events / error log)
-- ----------------------------------------------------------------
PRINT '';
PRINT '--- 15. Gợi ý bật Extended Events bắt query chậm (>500ms) ---';
PRINT '';
PRINT 'CREATE EVENT SESSION [Wcs_SlowQueries] ON SERVER';
PRINT 'ADD EVENT sqlserver.sql_statement_completed(';
PRINT '    ACTION(sqlserver.sql_text, sqlserver.client_app_name, sqlserver.database_name)';
PRINT '    WHERE (duration > 500000)';
PRINT ')';
PRINT 'ADD TARGET package0.event_file(SET filename=N''C:\Temp\WcsSlowQueries.xel'')';
PRINT 'WITH (MAX_MEMORY=4096 KB, EVENT_RETENTION_MODE=ALLOW_SINGLE_EVENT_LOSS);';
PRINT 'ALTER EVENT SESSION [Wcs_SlowQueries] ON SERVER STATE = START;';
PRINT '';
PRINT '================================================================';
PRINT ' Health check hoàn tất.';
PRINT '================================================================';
