![CI](https://github.com/leherv/ReleaseNotifier/actions/workflows/build.yml/badge.svg)
![Test_Results](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/leherv/f3101ad56d43a3586c957e2d6a36e458/raw/testresults.json)

![Deploy_PROD](https://github.com/leherv/ReleaseNotifier/actions/workflows/deploy_PROD.yml/badge.svg)
![Deploy_DEV](https://github.com/leherv/ReleaseNotifier/actions/workflows/deploy_heroku_DEV.yml/badge.svg)
[![GitHub issues](https://img.shields.io/github/issues/leherv/ReleaseNotifier)](https://github.com/leherv/ReleaseNotifier/issues)




# Release Notifier (RN)

## Local Development
Run the app locally
> dotnet run

and only start the database via docker 
> docker-compose -f ./build/docker-compose.yml -f ./build/docker-compose.DEV.yml up release_notifier_db --build --abort-on-container-exit --force-recreate

Do not forget that to set up playwright locally if you do not use docker. The simplest way is uncommenting the following line in PlaywrightScraper.cs:
> Program.Main(new[] {"install"});

this is not necessary when using Docker, as the image already contains Playwright.

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
## TODOs
* It would be nice to use IQuery<TQueryResult>/ICommand<TCommandResult> so the generics do not have to be specified each time
  * MediatR does this but the Dispatchers seem to have to be way more complicated
    * see discussion (https://cezarypiatek.github.io/post/why-i-dont-use-mediatr-for-cqrs/)
  * rethink application wide errors https://enterprisecraftsmanship.com/posts/advanced-error-handling-techniques/
* Use a more functional approach in the Application services (Handlers), like csharpfunctionalextensions

### Due to Heroku
Transform ASP.NET Core ReleaseNotifierApp to a console app as hosting an admin interface from the same app (by using process type web) is not possible as the 
web dyno idles after 30 minutes. The original plan was to do it in the same app... Maybe we will stick with the current setup and host somewhere else when/should
an interface follow.

Possible solution with heroku: We could create a second app ReleaseNotifierWebApp after transforming ReleaseNotifierApp to console based. This WebApp 
defines only the parts needed for the web interface in a second Dockerfile which is then deployed to Heroku as well. (so completely or almost completely different app but hosted in the same heroku app)

Maybe add an additional Dockerfile instead of copying it to the root. Heroku CLI in deploy_heroku_*.yml does not
support targeting a specific stage of the Dockerfile. Therefore unnecessary stages are executed when building the image
on github. Also it is not possible to call docker build from the project root when building it with Heroku CLI therefore
the copy step is currently necessary.

