@echo off
title ж�� AntCsScriptRun ����
color 04
echo ��ǰ�̷���%~d0
echo ��ǰ·����%~dp0  
echo ***********************  START  ***********************

echo ֹͣ AntCsScriptRun ����...

set current_path=%~dp0AntCsScriptRun\

%current_path%AntCsScriptRun.exe stop

echo ִ��ж�� AntCsScriptRun  ����...

sc delete AntCsScriptRun

pause 