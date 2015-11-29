#!/bin/bash

if [ "$1" == "sql" ]
then
    echo "Checking SQLite3 presence.."
    if grep -Fxq "deb http://ftp.us.debian.org/debian jessie main" /etc/apt/sources.list
    then
        echo "Source already present"
    else
        echo "Source not present.. Adding"
        echo "deb http://ftp.us.debian.org/debian jessie main" >> /etc/apt/sources.list
        apt-get update
        apt-get -y install sqlite3 libsqlite3-dev
    fi
elif [ "$1" == "code" ]
then
    echo "Updating code for deployment.."
    sed -i 's~options => { options\.UseSqlite(\$"Data Source={_appEnv\.ApplicationBasePath}/data\.db"); });~options => { options\.UseSqlite(\$"Data Source=/data/data\.db"); });~' /app/Startup.cs
    sed -i 's~"web": "Microsoft\.AspNet\.Server\.Kestrel",~"web": "Microsoft\.AspNet\.Server\.Kestrel --server\.urls http://marcelolima\.org",~' /app/project.json
elif [ "$1" == "push" ]
then
    echo "Pushing new files to server.."

    RUNNING=$(docker inspect --format="{{ .State.Running }}" sync 2> /dev/null)

    if [ $? -eq 1 ]; then
        echo "Container sync does not exist.. Creating"
        docker run -v ~/code/SyncTube/src/SyncTube/data:/data -d --name sync -p 80:80 sync
    elif [ "$RUNNING" == "false" ]; then
        echo "Container sync is not running.. Starting"
        docker start sync
    fi

    docker exec sync rm -rf *
    cd ~/code/SyncTube/src/SyncTube/
    docker cp . sync:/app
    cd -
    docker exec sync /app/configureDocker.sh code
#    docker restart sync
fi
