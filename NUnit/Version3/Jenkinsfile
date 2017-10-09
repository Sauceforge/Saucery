pipeline {
	agent any
	stages {
        stage('Clean') {
            steps {
				bat "\"${tool 'MSBuild'}\" NUnit\\Version3\\Saucery3.sln /t:Clean /p:Configuration=Release"
            }
        }
		stage('Rebuild') {
            steps {
				bat 'nuget restore NUnit\\Version3\\Saucery3.sln'
				bat "\"${tool 'MSBuild'}\" NUnit\\Version3\\Saucery3.sln /t:Rebuild /p:Configuration=Release"
            }
        }
		stage('UnitTest') {
            steps {
				bat '"C:\\Program Files (x86)\\NUnit.org\\nunit-console\\nunit3-console.exe" "NUnit\\Version3\\UnitTests\\bin\\Release\\UnitTests.dll" --result:junit-unittests-testsuite.xml;transform=nunit3-junit.xslt --trace=Verbose'
            }
        }
				
		stage('SauceTest') {
            steps {
				bat '"C:\\Program Files (x86)\\NUnit.org\\nunit-console\\nunit3-console.exe" "NUnit\\Version3\\Saucery3Tester\\bin\\Release\\Saucery3Tester.dll" --result:junit-selenium-testsuite.xml;transform=nunit3-junit.xslt --trace=Verbose'
            }
        }	
    }
	
	post {
      always {
        junit 'junit-*-testsuite.xml'
      }
   } 
}