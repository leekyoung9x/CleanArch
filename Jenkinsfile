pipeline {
  agent any

  environment {
        CURL_HOME = 'C:\\curl\\bin'  // Đường dẫn đến thư mục chứa curl
        PATH = "${CURL_HOME};${env.PATH}"  // Thêm cURL vào PATH
      }
  
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

    stage('Zip') {
      steps {
        bat '"C:\\Program Files\\7-Zip\\7z.exe" a "webapi-${params.BUILD_VERSION}.zip" "CleanArch.Api/bin/Release/net8.0"'
      }
    }

    stage('Upload to FTP') {
      steps {
        script {
          def ftpDetails = [
            url      : 'ftp://14.225.209.84:21',  // URL của server FTP
            username : 'administrator',                // Tên người dùng FTP
            password : 'fe64be2a-6e65-11ef-8417-00505690ef05',                // Mật khẩu của FTP
            remoteDir: ''             // Đường dẫn thư mục trên FTP
          ]
          // Sử dụng lệnh curl để đẩy file 'build.zip' lên server FTP
          bat """
          curl --ftp-port - -T build-${params.BUILD_VERSION}.zip -u ${ftpDetails.username}:${ftpDetails.password} ${ftpDetails.url}${ftpDetails.remoteDir}
          """
        }

      }
    }

  }
  environment {
    CURL_HOME = 'C:\\\\curl\\\\bin'
  }
  parameters {
    string(name: 'BUILD_VERSION', defaultValue: '1.0.0', description: 'Version number for the build')
  }
}
