@echo off
title 启动 AntCsScriptRun 服务
color 04
echo 当前盘符：%~d0
echo 当前路径：%~dp0AntCsScriptRun\
echo ***********************  START  ***********************

echo 启动 AntCsScriptRun 服务...

set current_path=%~dp0AntCsScriptRun\

%current_path%AntCsScriptRun.exe start

pause 