@echo off
title ���� AntCsScriptRun ����
color 04
echo ��ǰ�̷���%~d0
echo ��ǰ·����%~dp0AntCsScriptRun\
echo ***********************  START  ***********************

echo ���� AntCsScriptRun ����...

set current_path=%~dp0AntCsScriptRun\

%current_path%AntCsScriptRun.exe start

pause 