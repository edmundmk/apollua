@echo off
type NUL > test.txt
FOR %%F IN (Test\*.lua) DO bin\Debug\Test %%F >> test.txt
