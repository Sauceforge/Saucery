SET "RAWOUTPUT=raw_coverage.xml"
SET "FINALOUTPUT=outputCobertura.xml"
SET "TARGETDIR=CodeCoverageHTML"
DEL %RAWOUTPUT%
DEL %FINALOUTPUT%
RMDIR %TARGETDIR% /S /Q

NUnit\Version2\packages\OpenCover.4.5.3723\OpenCover.Console.exe -register:Path64 -target:NUnit\Version3\JenkinsScripts\RunUnitTests.bat -filter:"+[*]* -[*.UnitTests]* -[*.Tests]* -[*]*.Entities.* -[*]*.Properties.*" -output:%RAWOUTPUT% -hideskipped:All
NUnit\Version2\packages\ReportGenerator.2.1.6.0\ReportGenerator.exe -reports:%RAWOUTPUT% -targetDir:%TARGETDIR%
NUnit\Version2\packages\OpenCoverToCoberturaConverter.0.2.3.0\OpenCoverToCoberturaConverter.exe -input:%RAWOUTPUT% -output:%FINALOUTPUT% -sources:%WORKSPACE%