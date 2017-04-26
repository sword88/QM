update sapsr3.ZTPPSTAGE013@sap_prd set MANDT ='100' where mandt='1016'  and rownum < 500;
commit;
exit;