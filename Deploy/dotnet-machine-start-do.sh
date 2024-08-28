echo "---- UPDATE DA MAQUINA ----"

sudo apt-get -y update

echo "---- KILL APPLICATION ----"

sudo pkill -f 'dotnet SatelittiBpms.dll'

echo "---- CLEAN APPLICATION FOLDER ----"

sudo rm -rf SatelittiBpms

echo "---- INSTALL .NET CORE ----"

wget https://packages.microsoft.com/config/ubuntu/20.10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

sudo apt-get install -y apt-transport-https
sudo apt-get update
sudo apt-get install -y dotnet-sdk-6.0

echo "---- INSTALL JQ ----"

sudo apt-get install -y jq

echo "---- GET VARIABLES ----"

instance=$(curl http://169.254.169.254/latest/meta-data/instance-id)
region=$(curl http://169.254.169.254/latest/meta-data/placement/availability-zone)
module=$(aws ec2 describe-tags --region ${region:0:9}|jq -r ".Tags[] | select(.ResourceId==\"$instance\") | select(.Key==\"module\").Value")
environment=$(aws ec2 describe-tags --region ${region:0:9}|jq -r ".Tags[] | select(.ResourceId==\"$instance\") | select(.Key==\"environment\").Value")
version=$(aws ec2 describe-tags --region ${region:0:9}|jq -r ".Tags[] | select(.ResourceId==\"$instance\") | select(.Key==\"version\").Value")
echo $module/$environment/$version

echo "---- DOWNLOAD APPLICATION ----"

sudo aws s3 cp s3://com.selbetti.sportal.deploy/deploy/$module/$environment/$version/ SatelittiBpms/ --recursive --region us-east-1
sudo aws s3 cp s3://com.selbetti.sportal.deploy/configuration/$module/$environment/aspnetcore_envvars.sh /etc/profile.d/aspnetcore_envvars.sh --region us-east-1
sudo dos2unix /etc/profile.d/aspnetcore_envvars.sh
source /etc/profile.d/aspnetcore_envvars.sh

echo "---- START APPLICATION ----"

cd SatelittiBpms/
nohup sudo ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT ASPNETCORE_URLS=$ASPNETCORE_URLS dotnet SatelittiBpms.dll > /dev/null &

echo "---- END SH ----"