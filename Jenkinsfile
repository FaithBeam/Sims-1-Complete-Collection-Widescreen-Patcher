def VERSION
pipeline {
    agent 'any'
    stages {
        stage('Establish Variables') {
            steps {
                script {
                    VERSION = powershell(returnStdout: true, script: '''(Get-Content .\\Sims1WidescreenPatcher\\Properties\\AssemblyInfo.cs | Select-String -Pattern \'AssemblyInformationalVersion\\(\\"(.+)-d\\"\\)\').Matches.Groups[1].Value''')
                }
            }
        }
        stage('Nuget Restore') {
            steps {
                bat 'nuget restore'
            }
        }
        stage('Build') {
            steps {
                powershell 'msbuild .\\Sims1WidescreenPatcher.sln -p:Configuration=Release -p:OutDir=\"\$(\$pwd)\\bin\"'
            }
        }
        stage('Pack') {
            steps {
                powershell "Compress-Archive -Path .\\* -DestinationPath \"\$(\$HOME)\\Desktop\\Sims1WidescreenPatcher.${VERSION}.zip\""
            }
        }
    }
}