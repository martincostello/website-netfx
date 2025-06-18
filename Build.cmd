@ECHO OFF

"%ProgramFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\msbuild" "%~dp0\martincostello.com.msbuild" /v:minimal /maxcpucount /nodeReuse:false %*
