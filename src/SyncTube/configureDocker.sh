#!/bin/bash

if [ "$1" == "sql" ]
then
    if grep -Fxq "deb http://ftp.us.debian.org/debian jessie main" /etc/apt/sources.list
    then
        echo "Source already present"
    else
        echo "Source not present.. Adding"
#        echo "deb http://ftp.us.debian.org/debian jessie main" >> /etc/apt/sources.list
#        apt-get update
#        apt-get -y install sqlite3 libsqlite3-dev
    fi
elif [ "$1" == "code" ]
then
    echo "Updating code for deployment"
    
fi

