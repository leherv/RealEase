FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base
WORKDIR /app
COPY . ./
RUN dotnet restore

FROM base as test
RUN apt-get update && \
    apt-get install -y netcat 
CMD sh -c 'chmod +x ./build/*.sh; ./build/wait-for.sh release_notifier_db:5432 -t 30 -- dotnet test --results-directory "test-results" --logger trx --no-restore'

FROM base as publish
COPY --from=base app/build/ out/
#RUN pwsh /out/bin/Debug/netX/playwright.ps1 install
RUN dotnet publish -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as run
#RUN apt-get update && apt-get -f install && apt-get -y install wget gnupg2 apt-utils
#RUN apt-get install -y chromium fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst \
#      --no-install-recommends
WORKDIR /app
COPY --from=publish /app/out .

FROM run as runDEV
RUN apt-get update && \
apt-get install -y netcat 
CMD sh -c 'chmod +x *.sh; ./wait-for.sh release_notifier_db:5432 -t 30 -- dotnet ReleaseNotifierApp.dll'

FROM run as runPROD
CMD ["dotnet", "ReleaseNotifierApp.dll"]
