@echo off
title 卸载 AntCsScriptRun 服务
color 04
echo 当前盘符：%~d0
echo 当前路径：%~dp0  
echo ***********************  START  ***********************

echo 停止 AntCsScriptRun 服务...

set current_path=%~dp0AntCsScriptRun\

%current_path%AntCsScriptRun.exe stop

echo 执行卸载 AntCsScriptRun  服务...

sc delete AntCsScriptRun

pause 