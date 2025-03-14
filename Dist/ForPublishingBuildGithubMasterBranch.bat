set OUTFILE=BizHawk-Github-Master.zip
set WORKDIR=.build-github-master

set PATH=%PATH%;C:\Program Files (x86)\git\bin;C:\Program Files\git\bin

rem cleanup old working and output files
del ..\..\%OUTFILE%
rmdir /s /q %WORKDIR%

rem prepare working directory for clone
mkdir %WORKDIR%

rem http://stackoverflow.com/questions/13750182/git-how-to-archive-from-remote-repository-directly
cd %WORKDIR%
git clone --depth=1 --single-branch --branch master https://github.com/TASEmulators/BizHawk.git .

rem use updated build scripts
copy /y ..\QuickTestBuildAndPackage_Release.bat dist
copy /y ..\QuickTestBuildAndPackage.bat dist

rem build the branch
cd dist
call QuickTestBuildAndPackage_Release.bat

rem make sure we save the output!
move BizHawk.zip ..\..\%OUTFILE%

rem cleanup
cd ..\..
rmdir /s /q %WORKDIR%
