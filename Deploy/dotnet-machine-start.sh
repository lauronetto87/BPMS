aws s3 cp s3://com.selbetti.sportal.deploy/sh/bpms/dotnet-machine-start-do.sh dotnet-machine-start-do.sh --region us-east-1
chmod +x dotnet-machine-start-do.sh
dos2unix dotnet-machine-start-do.sh
./dotnet-machine-start-do.sh > start-do.log