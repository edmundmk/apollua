@echo off
type NUL > test.txt
FOR %%F IN (test\*.luac) DO Lua\bin\Debug\Lua %%F >> test.txt