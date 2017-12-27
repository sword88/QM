/* Formatted on 2017/11/3 8:17:31 (QP5 v5.252.13127.32847) */
--定义列名
COLUMN TC NEW_VALUE FILENAME;
COLUMN STRTIME NEW_VALUE STIME;
COLUMN ENDTIME NEW_VALUE ETIME;
--定义变量名
VARIABLE    v_PO       VARCHAR2 (120);                                      
VARIABLE    v_POITEM   VARCHAR2 (400);                                   
VARIABLE    v_AO       VARCHAR2 (120);                              
VARIABLE    V_SQLERR   varchar2(1000);
--定义输出日志名及路径

SELECT TO_CHAR (SYSDATE, 'MMDD_hh24') || '_rpt.txt' TC FROM DUAL;

SPOOL  C:\QM\Files\2017\11\&FILENAME;
--启用DBMS OUTPUT
SET SERVEROUTPUT ON;
--程序开始时间

SELECT '''' || TO_CHAR (SYSDATE, 'mm-dd hh24:mi:ss') || '''' STRTIME
  FROM DUAL;

declare   CURSOR CUR_ASD
   IS
    SELECT PO, POITEM, AOLOT
        FROM WO_PO@MESDB1
       WHERE CONTROL IN (SELECT DISTINCT B.CONTROLLOTID
                           FROM WO_PO A, FWCATNS_STG_AOLOT B
                          WHERE     A.PO IS NULL
                                AND TO_CHAR (a.cdt, 'yyyy') = '2017'
                                AND A.AOLOT = B.AOLOTID);

C_ASD      CUR_ASD%ROWTYPE;


BEGIN
   FOR C_ASD IN CUR_ASD
   LOOP
      EXIT WHEN CUR_ASD%NOTFOUND;
      :v_PO := C_ASD.PO;
      :v_POITEM := C_ASD.POITEM;
      :v_aO := C_ASD.AOLOT;

      UPDATE WO_PO
         SET PO = :v_PO, POITEM = :v_POITEM
       WHERE AOLOT = :v_aO;
	  COMMIT;
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