del simian-output.xml /Q
"C:\Program Files (x86)\Simian\bin\simian-2.3.35" **/*.cs -formatter=xml:simian-output.xml -failOnDuplication- -reportDuplicateText+ -excludes="**/*.designer.cs" -excludes="**/*_partial.cs" -excludes="**/*Tests.cs" -excludes="**/*Reference.cs" -excludes=Tools/**/*.cs
exit %%ERRORLEVEL%%