/* Formatted on 2017/11/3 8:17:31 (QP5 v5.252.13127.32847) */
--定义列名
COLUMN TC NEW_VALUE FILENAME;
COLUMN STRTIME NEW_VALUE STIME;
COLUMN ENDTIME NEW_VALUE ETIME;
--定义变量名
VARIABLE    V_ICNT     varchar2(1000);    
VARIABLE    V_PID      varchar2(1000);                                                              
VARIABLE    V_SQLERR   varchar2(1000);
--定义输出日志名及路径

SELECT TO_CHAR (SYSDATE, 'MMDD_hh24') || '_rpt.txt' TC FROM DUAL;

SPOOL  C:\qm\&FILENAME;
--启用DBMS OUTPUT
SET SERVEROUTPUT ON;
--程序开始时间

SELECT '''' || TO_CHAR (SYSDATE, 'mm-dd hh24:mi:ss') || '''' STRTIME
  FROM DUAL;

   declare CURSOR MY_CURSOR
   IS
      SELECT AUFNR
        FROM SAPSR3.AUFK@SAP_PRD
       WHERE AUART = 'PP08' AND ERDAT <= '20161231' AND ERDAT >= '20160531';

   C_CURSOR   MY_CURSOR%ROWTYPE;


BEGIN
   FOR C_CURSOR IN MY_CURSOR
   LOOP		
      SELECT NVL(PCNT,99) into
         :V_ICNT
        FROM (SELECT (LEAD (CNT, 1) OVER (ORDER BY VORNR DESC) - A.CNT) PCNT
                FROM (  SELECT VORNR, SUM (GMNGA) + SUM (XMNGA) CNT
                          FROM SAPSR3.AFRU@SAP_PRD
                         WHERE     MANUR = '1'
                               AND MEILR = 'X'
							   and ltxa1 <>'Reason for Cancellation'
                               and stokz <> 'X'
                               AND AUFNR = C_CURSOR.AUFNR
                      GROUP BY VORNR) A)
       WHERE ROWNUM = 1;

      IF :V_ICNT != '0'
      THEN
		SELECT PLNBEZ INTO :V_PID from sapsr3.afko@SAP_PRD where aufnr =C_CURSOR.AUFNR;
	  
         INSERT INTO MESFT.CXH ("BDNO","CUS_FGDEVICE")
              VALUES (C_CURSOR.AUFNR,:V_PID);

         COMMIT;
      END IF;
   END LOOP;
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