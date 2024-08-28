Content-Type: multipart/mixed; boundary="//"
MIME-Version: 1.0

--//
Content-Type: text/cloud-config; charset="us-ascii"
MIME-Version: 1.0
Content-Transfer-Encoding: 7bit
Content-Disposition: attachment; filename="cloud-config.txt"

#cloud-config
cloud_final_modules:
- [scripts-user, always]

--//
Content-Type: text/x-shellscript; charset="us-ascii"
MIME-Version: 1.0
Content-Transfer-Encoding: 7bit
Content-Disposition: attachment; filename="userdata.txt"

#!/bin/bash
apt-get -y update
apt-get install -y dos2unix
apt-get install -y awscli

cd /home/ubuntu
aws s3 cp s3://com.selbetti.sportal.deploy/sh/bpms/dotnet-machine-start.sh dotnet-machine-start.sh --region us-east-1
chmod +x dotnet-machine-start.sh
dos2unix dotnet-machine-start.sh
./dotnet-machine-start.sh
--//