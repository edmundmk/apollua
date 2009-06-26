@echo off
type NUL > test.txt
FOR %%F IN (Test\*.lua) DO bin\Release\Test %%F >> test.txt
