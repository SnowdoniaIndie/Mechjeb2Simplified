
IF NOT DEFINED KSPDIR echo KSPDIR env variable not defined

REM Strip the surrounding quotes MSBuild adds, so paths
REM containing spaces re-quote correctly below.
SET TargetPath=%~1
SET TargetDir=%~2
SET TargetName=%~3
SET ProjectDir=%~4

echo:
echo Target Path: %TargetPath%
echo Target Dir: %TargetDir%
echo Target Name: %TargetName%
echo Project Dir: %ProjectDir%
echo:
echo kspdir: "%KSPDIR%"

IF NOT EXIST "%KSPDIR%\*" (
	echo Expected "%KSPDIR%" to point to a directory but it is not
	exit 0
)

echo:
echo Copying to "%KSPDIR%"
echo:

SET PLUGINS=%KSPDIR%\GameData\MechJeb2\Plugins\
IF NOT EXIST "%PLUGINS%" mkdir "%PLUGINS%"

IF EXIST "%TargetPath%" xcopy /Y /I "%TargetPath%" "%PLUGINS%"
IF EXIST "%TargetDir%%TargetName%.pdb" xcopy /Y /I "%TargetDir%%TargetName%.pdb" "%PLUGINS%"
IF EXIST "%TargetDir%JetBrains.Annotations.dll" xcopy /Y /I "%TargetDir%JetBrains.Annotations.dll" "%PLUGINS%"
IF EXIST "%TargetDir%JetBrains.Annotations.xml" xcopy /Y /I "%TargetDir%JetBrains.Annotations.xml" "%PLUGINS%"
IF EXIST "%TargetDir%MechJebLib.dll" xcopy /Y /I "%TargetDir%MechJebLib.dll" "%PLUGINS%"
IF EXIST "%TargetDir%MechJebLib.pdb" xcopy /Y /I "%TargetDir%MechJebLib.pdb" "%PLUGINS%"
IF EXIST "%TargetDir%MechJebLibBindings.dll" xcopy /Y /I "%TargetDir%MechJebLibBindings.dll" "%PLUGINS%"
IF EXIST "%TargetDir%MechJebLibBindings.pdb" xcopy /Y /I "%TargetDir%MechJebLibBindings.pdb" "%PLUGINS%"
IF EXIST "%TargetDir%alglib.dll" xcopy /Y /I "%TargetDir%alglib.dll" "%PLUGINS%"
IF EXIST "%TargetDir%alglib.pdb" xcopy /Y /I "%TargetDir%alglib.pdb" "%PLUGINS%"

IF EXIST "%ProjectDir%..\Bundles" xcopy /S /Y /I "%ProjectDir%..\Bundles" "%KSPDIR%\GameData\MechJeb2\Bundles"
IF EXIST "%ProjectDir%..\Icons" xcopy /S /Y /I "%ProjectDir%..\Icons" "%KSPDIR%\GameData\MechJeb2\Icons"
IF EXIST "%ProjectDir%..\Localization" xcopy /S /Y /I "%ProjectDir%..\Localization" "%KSPDIR%\GameData\MechJeb2\Localization"
IF EXIST "%ProjectDir%..\Parts" xcopy /S /Y /I "%ProjectDir%..\Parts" "%KSPDIR%\GameData\MechJeb2\Parts"
IF EXIST "%ProjectDir%..\LandingSites.cfg" xcopy /Y /I "%ProjectDir%..\LandingSites.cfg" "%KSPDIR%\GameData\MechJeb2\"

REM SimpleAutopilot: deploy our ModuleManager patch, toolbar icons + AVC version file
IF EXIST "%ProjectDir%SimpleAutopilot\SimpleAutopilot.cfg" xcopy /Y /I "%ProjectDir%SimpleAutopilot\SimpleAutopilot.cfg" "%KSPDIR%\GameData\MechJeb2\SimpleAutopilot\"
IF EXIST "%ProjectDir%SimpleAutopilot\Icons" xcopy /S /Y /I "%ProjectDir%SimpleAutopilot\Icons" "%KSPDIR%\GameData\MechJeb2\SimpleAutopilot\Icons"
IF EXIST "%ProjectDir%SimpleAutopilot\MechJeb2Simplified.version" xcopy /Y /I "%ProjectDir%SimpleAutopilot\MechJeb2Simplified.version" "%KSPDIR%\GameData\MechJeb2\SimpleAutopilot\"

echo:
:: Display the time of compilation so I don't waste time testing code I did not compile...
time /t
