# Release Notifier (RN)

## Local Development
Run the app locally
> dotnet run

and only start the database via docker 
> docker-compose -f ./build/docker-compose.yml -f ./build/docker-compose.DEV.yml up release_notifier_db --build --abort-on-container-exit --force-recreate

Run all tests
> dotnet test

Run integration tests only
> dotnet test --filter=Category=Integration

Run only unit tests
> dotnet test --filter=Category!=Integration

Running everything in docker is also possible
> docker-compose -f ./build/docker-compose.yml -f ./build/docker-compose.DEV.yml up --build --abort-on-container-exit --force-recreate

Before each PR, check if everything works with docker
> docker-compose -f ./build/docker-compose.yml -f ./build/docker-compose.CI.yml up --build --abort-on-container-exit --force-recreate


## PROD
> docker-compose -f ./build/docker-compose.yml up --build --abort-on-container-exit --force-recreate


## Troubleshooting
Build specific stage from the Dockerfile in /build
> docker build -t release_notifier -f .\build\Dockerfile --target [targetName] .

Then use
> docker run -t -d release_notifier

to start the container and keep it running so e.g. portainer can be used to look inside.


## General 
***Note: all commands are executed from the project root***

Values for the environment variables mentioned in the docker-compose files have to be supplied. This can be done via .env file, replacing
the ${} with values directly or by setting environment values on the machine.

___
TODOs:
* It would be nice to use IQuery<TQueryResult>/ICommand<TCommandResult> so the generics do not have to be specified each time
  * MediatR does this but the Dispatchers seem to have to be way more complicated
    * see discussion (https://cezarypiatek.github.io/post/why-i-dont-use-mediatr-for-cqrs/)
* Aggregate creation errors in the domain and return to Application services (or use fail-fast with results)
* Define ApplicationResult maybe using the same Result classes used in the domain?
  * use application wide errors https://enterprisecraftsmanship.com/posts/advanced-error-handling-techniques/
    * Remove exception gunk


* and of course also functional requirements
  * adding a media via workflow (defining name, sources (ScrapeEndpoints))
  * unsubscribing