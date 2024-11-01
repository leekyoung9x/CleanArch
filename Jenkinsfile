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

  }
}