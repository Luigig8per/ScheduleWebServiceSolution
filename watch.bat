@echo off
@color 0a
echo Please do not close this window

:loop

title "DonBest Schedule - Run Don Best Schedule each 24 hoours"

echo Running Don Best Schedule
start /min cmd /k  "ScheduleWebServiceSolution.exe /min" 

timeout /t 400
echo Don best Schedule executed. If you want run Don Best Schedule again now, just press enter, it will run now and after 24 hours will run again
timeout /t 86000
goto loop


