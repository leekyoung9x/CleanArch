pipeline {
  agent any
  stages {
    stage('') {
      steps {
        script {
          currentBuild.displayName = "${params.BUILD_VERSION}"
        }

        bat 'dotnet --version'
      }
    }

  }
}