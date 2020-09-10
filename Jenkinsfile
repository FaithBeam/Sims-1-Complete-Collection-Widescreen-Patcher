def VERSION
pipeline {
    agent 'any'
    stages {
        stage('Establish Variables') {
            steps {
                script {
                    VERSION = powershell(returnStdout: true, script: '''(Get-Content .\\Sims1WidescreenPatcher\\Properties\\AssemblyInfo.cs | Select-String -Pattern \'AssemblyVersion\\(\\"(\\d+\\.\\d+\\.\\d+).+\\)]\').Matches.Groups[1].Value''').trim()
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
                powershell "Compress-Archive -Path .\\bin\\* -DestinationPath \"\$(\$HOME)\\Desktop\\Sims1WidescreenPatcher.${VERSION}+\$(\$env:GIT_COMMIT.Substring(0,7)).zip\""
            }
        }
    }
}