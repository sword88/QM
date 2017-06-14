/***********************************************************************
 模拟DB 操作
***********************************************************************/ 

--定义列名
COLUMN TC NEW_VALUE FileName;
COLUMN STRTIME NEW_VALUE STIME;
COLUMN ENDTIME NEW_VALUE ETIME;
--定义变量名
Variable IDX   VARCHAR2 (100);
Variable v_SQLERR varchar2(255);
--定义输出日志名及路径
SELECT TO_CHAR(SYSDATE,'MMDD_hh24') || '_rpt.txt' TC FROM DUAL;
spool  c:\qm\&FileName;
--启用DBMS OUTPUT
set serveroutput on;
--程序开始时间
select ''''||to_char(sysdate,'mm-dd hh24:mi:ss')||'''' STRTIME from dual;

--程序主体
BEGIN
   SELECT IT_SEQ.NEXTVAL INTO :IDX FROM DUAL;

   INSERT INTO QM_TASKLOG 
        VALUES (:IDX,
                :IDX || 'ID',
                'NA',
                SYSDATE,
                :IDX || 'MESSAGE');

   COMMIT;
   --模拟操作，休眠10秒
   --注意使用权限
   DBMS_LOCK.SLEEP(10);
   --异常输出
   Exception
when others then 
  :v_SQLERR:=substr(sqlerrm,1,255);
  DBMS_OUTPUT.PUT_LINE(:v_SQLERR);
END;
/
--程序结束时间
select ''''||to_char(sysdate,'mm-dd hh24:mi:ss')||'''' ENDTIME from dual;
select trunc(to_number(to_date(&etime,'mm-dd hh24:mi:ss')-to_date(&stime,'mm-dd hh24:mi:ss')) * 24 *60 ,2) || 'Mins' total from dual;
set serveroutput off;
SPOOL OFF;
EXIT;