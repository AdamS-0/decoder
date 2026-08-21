@echo off
set vnFile=../../vn
set /p vn=<%vnFile%
set /a vn=(%vn% + 1)
echo New version number : [%vn%]
echo %vn% > %vnFile%

set dD=%DATE:~0,2%
set dM=%DATE:~3,2%
set dY=%DATE:~8,2%

set tH=%TIME:~0,2%
set tM=%TIME:~3,2%
set tS=%TIME:~7,2%

set src=..\..\vn.cs
echo using System; > %src%
echo namespace decoder { >> %src%
echo     public class vn { >> %src%
echo         public const string BUILD_NUM  = "%vn%"; >> %src%
echo         public const string BUILD_YEAR = "%dY%"; >> %src%
echo         public const string BUILD_MON  = "%dM%"; >> %src%
echo         public const string BUILD_DAY  = "%dD%"; >> %src%
echo         public const string BUILD_HOUR = "%tH%"; >> %src%
echo         public const string BUILD_MIN  = "%tM%"; >> %src%
echo         public vn() {} >> %src%
echo     } >> %src%
echo } >> %src%
