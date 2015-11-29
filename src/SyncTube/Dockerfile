FROM microsoft/aspnet:1.0.0-beta8-coreclr

COPY project.json /app/
COPY NuGet.config /app/
COPY configureDocker.sh /app/
WORKDIR /app
RUN ["dnu", "restore"]
RUN ["./configureDocker.sh", "sql"]
RUN apt-get update
RUN apt-get -y install sqlite3 libsqlite3-dev

COPY . /app
VOLUME /data

RUN ["./configureDocker.sh", "code"]

EXPOSE 80
ENTRYPOINT ["dnx", "-p", "project.json", "web"]
