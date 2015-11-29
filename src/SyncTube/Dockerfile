FROM microsoft/aspnet:1.0.0-beta8-coreclr

COPY project.json /app/
COPY ../../NuGet.config /app/
COPY configureDocker.sh /app/
WORKDIR /app
RUN ["dnu", "restore"]
RUN ./configureDocker.sh
RUN apt-get update
RUN apt-get -y install sqlite3 libsqlite3-dev

COPY . /app
RUN sed -ie 's/options => { options.UseSqlite($"Data Source={_appEnv.ApplicationBasePath}/data.db"); });/options => { options.UseSqlite($"Data Source=/data/data.db"); });/' /app/Startup.cs
VOLUME /data

EXPOSE 80
ENTRYPOINT ["dnx", "-p", "project.json", "web"]
