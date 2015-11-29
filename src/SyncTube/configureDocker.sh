#!/bin/bash

if [ "$1" == "sql" ]
then
    echo -e "\e[33m>> Checking SQLite3 presence..\e[m"
#echo -e "\e[93mPlease specify target\e[0m"
    if grep -Fxq "deb http://ftp.us.debian.org/debian jessie main" /etc/apt/sources.list
    then
        echo -e "\e[32m>> Source already present\e[m"
    else
        echo -e "\e[91m>> Source not present.. Adding\e[m"
        echo "deb http://ftp.us.debian.org/debian jessie main" >> /etc/apt/sources.list
        apt-get update
        apt-get -y install sqlite3 libsqlite3-dev
    fi
elif [ "$1" == "code" ]
then
    echo -e "\e[33m>> Updating code for deployment..\e[m"
    sed -i 's~options => { options\.UseSqlite(\$"Data Source={_appEnv\.ApplicationBasePath}/data\.db"); });~options => { options\.UseSqlite(\$"Data Source=/data/data\.db"); });~' /app/Startup.cs
    sed -i 's~"web": "Microsoft\.AspNet\.Server\.Kestrel",~"web": "Microsoft\.AspNet\.Server\.Kestrel --server\.urls http://marcelolima\.org",~' /app/project.json
    echo -e "\e[32m>> Updated!\e[m"
elif [ "$1" == "push" ]
then
    echo -e "\e[33m>> Pushing new files to server..\e[m"

    RUNNING=$(docker inspect --format="{{ .State.Running }}" sync 2> /dev/null)

    if [ $? -eq 1 ]; then
        echo -e "\e[31m>> Container sync does not exist.. Creating\e[m"
        docker run -v ~/code/SyncTube/src/SyncTube/data:/data -d --name sync -p 80:80 sync
    elif [ "$RUNNING" == "false" ]; then
        echo -e "\e[31m>> Container sync is not running.. Starting\e[m"
        docker start sync
    fi

    echo -e "\e[33m>>  Cleaning app folder in container..\e[m"
    docker exec sync rm -rf *
    cd ~/code/SyncTube/src/SyncTube/
    echo -e "\e[33m>>  Pushing new version..\e[m"
    docker cp . sync:/app
    cd - > /dev/null
    docker exec sync /app/configureDocker.sh code
    echo -e "\e[33m>>  Restarting container..\e[m"
    docker restart sync
    echo -e "\e[32m>>  Push complete\e[m"
fi