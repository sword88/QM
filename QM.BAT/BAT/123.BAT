@md c:\qm\file\
@dir c:\qm\file\ /b /a-d | find /v /c "&#@" > c:\qm\file\count.log
@for /f "tokens=*" %%i in ('type c:\qm\file\count.log') do set /a a=%%i
echo > c:\qm\file\%a%.txt