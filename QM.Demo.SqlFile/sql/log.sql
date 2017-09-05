/***********************************************************************
 模拟DB 操作
***********************************************************************/

--定义列名
COLUMN TC NEW_VALUE FILENAME;
COLUMN STRTIME NEW_VALUE STIME;
COLUMN ENDTIME NEW_VALUE ETIME;
--定义变量名
VARIABLE IDX   VARCHAR2 (100);
VARIABLE V_SQLERR VARCHAR2(255);
--定义输出日志名及路径

SELECT TO_CHAR (SYSDATE, 'MMDD_hh24') || '_rpt.txt' TC FROM DUAL;

SPOOL  C:\QM\FILE\&FILENAME;
--启用DBMS OUTPUT
SET SERVEROUTPUT ON;
--程序开始时间

SELECT '''' || TO_CHAR (SYSDATE, 'mm-dd hh24:mi:ss') || '''' STRTIME
  FROM DUAL;

--程序主体

BEGIN
   SELECT IT_SEQ.NEXTVAL INTO :IDX FROM DUAL;

   INSERT INTO QM_TASKLOG
        VALUES ( :IDX,
                :IDX || 'ID',
                'NA',
                SYSDATE,
                :IDX || 'MESSAGE',
				'NA');

   COMMIT;
   --模拟操作，休眠10秒
   --注意使用权限
   DBMS_LOCK.SLEEP (1);
--异常输出
EXCEPTION
   WHEN OTHERS
   THEN
      :V_SQLERR := SUBSTR (SQLERRM, 1, 255);
      DBMS_OUTPUT.PUT_LINE ( :V_SQLERR);
END;
/

--程序结束时间

SELECT '''' || TO_CHAR (SYSDATE, 'mm-dd hh24:mi:ss') || '''' ENDTIME
  FROM DUAL;

SELECT    TRUNC (
               TO_NUMBER (
                    TO_DATE (&ETIME, 'mm-dd hh24:mi:ss')
                  - TO_DATE (&STIME, 'mm-dd hh24:mi:ss'))
             * 24
             * 60,
             2)
       || 'Mins'
          TOTAL
  FROM DUAL;

SET SERVEROUTPUT OFF;
SPOOL OFF;
--程序退出
EXIT;