def VERSION
pipeline {
    agent 'any'
    stages {
        stage('Establish Variables') {
            VERSION = powershell(eturnStdout: true, script: """
            (Get-Content .\Sims1WidescreenPatcher\Properties\AssemblyInfo.cs | Select-String -Pattern 'AssemblyInformationalVersion\(\"(.+)-d\"\)').Matches.Groups[1].Value
            """).trim()
        }
        stage('Nuget Restore') {
            bat 'nuget restore'
        }
        stage('Build') {
            bat 'msbuild .\Sims1WidescreenPatcher.sln -p:Configuration=Release -p:OutDir="$($pwd)\bin"'
        }
        stage('Pack') {
            powershell "Compress-Archive -Path .\* -DestinationPath \"$($HOME)\Desktop\Sims1WidescreenPatcher.${VERSION}.zip\""
        }
    }
}