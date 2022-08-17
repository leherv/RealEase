#TODO: improve build speed
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base
WORKDIR /app
COPY . ./
RUN dotnet restore

FROM base as test
RUN apt-get update && \
    apt-get install -y netcat &&\
    dotnet tool restore
CMD sh -c 'chmod +x ./build/*.sh; \
           ./build/wait-for.sh real_ease_db:5432 -t 30 -- dotnet test --configuration Release --verbosity normal --collect:"XPlat Code Coverage" --results-directory ./test-results --logger trx --no-restore; \
           dotnet reportgenerator "-reports:test-results/**/*.xml" "-targetdir:test-results" "-reporttypes:Cobertura;Badges"'

FROM base as publish
COPY --from=base app/build/ out/
RUN dotnet publish -c Release -o out --no-restore

FROM mcr.microsoft.com/playwright/dotnet:v1.21.0-focal as run
RUN apt-get update && apt-get -f install && apt-get -y install dumb-init
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/out .
#(to avoid zombie-processes, see: https://github.com/Yelp/dumb-init and https://blog.phusion.nl/2015/01/20/docker-and-the-pid-1-zombie-reaping-problem/)
ENTRYPOINT ["/usr/bin/dumb-init", "--"]

FROM run as runDEV
RUN apt-get update && \
apt-get install -y netcat 
CMD sh -c 'chmod +x *.sh; ./wait-for.sh real_ease_db:5432 -t 30 -- dotnet RealEaseApp.dll'

FROM run as runPROD
CMD ["dotnet", "RealEaseApp.dll"]
