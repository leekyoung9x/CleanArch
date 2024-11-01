pipeline {
  agent any
  stages {
    stage('Set Build Display Name') {
      parallel {
        stage('Set Build Display Name') {
          steps {
            script {
              currentBuild.displayName = "${params.BUILD_VERSION}"
            }

          }
        }

        stage('Check dotnet version') {
          steps {
            bat 'dotnet --version'
          }
        }

      }
    }

    stage('Build') {
      steps {
        bat 'dotnet build -c Release'
      }
    }

    stage('Zip file') {
      steps {
        bat 'bat "\\"C:\\\\Program Files\\\\7-Zip\\\\7z.exe\\" a build-${params.BUILD_VERSION}.zip bin/release"'
      }
    }

  }
  parameters {
    string(name: 'BUILD_VERSION', defaultValue: '1.0.0', description: 'Version number for the build')
  }
}