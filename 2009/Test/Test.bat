@echo off
FOR %%F IN (Test\*.lua) DO bin\Debug\Test %%F > %%F.out.txt
