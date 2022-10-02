# RealEase

![CI](https://github.com/leherv/RealEase/actions/workflows/build.yml/badge.svg)
![Deploy_DEV](https://github.com/leherv/RealEase/actions/workflows/deploy_heroku_DEV.yml/badge.svg)
![Deploy_PROD](https://github.com/leherv/RealEase/actions/workflows/deploy_PROD.yml/badge.svg)

![Test_Results_success](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/leherv/f3101ad56d43a3586c957e2d6a36e458/raw/testresult_success.json&color=brightgreen)
![Test_Results_failed](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/leherv/f3101ad56d43a3586c957e2d6a36e458/raw/testresult_failed.json)
![Combined_Coverage](https://gist.githubusercontent.com/leherv/f3101ad56d43a3586c957e2d6a36e458/raw/8b55a6bd21ca1244e61786fb9610161843249c93/badge_combined.svg)

![Branch_Coverage](https://gist.githubusercontent.com/leherv/f3101ad56d43a3586c957e2d6a36e458/raw/8b55a6bd21ca1244e61786fb9610161843249c93/badge_branchcoverage.svg)
![Line_Coverage](https://gist.githubusercontent.com/leherv/f3101ad56d43a3586c957e2d6a36e458/raw/8b55a6bd21ca1244e61786fb9610161843249c93/badge_linecoverage.svg)
![Method_Coverage](https://gist.githubusercontent.com/leherv/f3101ad56d43a3586c957e2d6a36e458/raw/8b55a6bd21ca1244e61786fb9610161843249c93/badge_methodcoverage.svg)

[![GitHub issues](https://img.shields.io/github/issues/leherv/RealEase)](https://github.com/leherv/RealEase/issues)

## Get started

## Mutual server with bot
Add the bot to one of your servers if you are not already in a server with it. You can do this via the invite url here: [Invitation Link](https://discord.com/api/oauth2/authorize?client_id=962068918686609520&permissions=2048&redirect_uri=http%3A%2F%2Frealease.viktorleher.at%2Fsignin-discord&response_type=code&scope=identify%20bot)
You can also do this via the **[offical website](https://realease.viktorleher.at)**.

## Manage subscriptions
Either use the **[offical website](https://realease.viktorleher.at)(recommended)** to manage your subscriptions or just directly chat with the bot by using his **built in commands** (for all available commands type "re!h").
To use the official website you have to log in by using your Discord-Account (you will be redirected to Discord).
You can then:
* Subscribe to and unsubscribe from different media
* Search media by name
* Sort media by subscription status or name
* Add new media
* Add new Scrape Targets

## Add new media
By hitting the Add Media Button you will be prompted to choose one of the supported websites as well as provide a relative url path to the specific media you want to add. This media can then be found and subscribed to by all other users.

## Add new Scrape Target
For an already existing media it is possible to add additional Scrape Targets. You will again be prompted to choose a supported website as well as a relative link to the desired media on that website. All ScrapeTargets configured for a media will be checked for new releases. The one adding the new release first will be used for the notification.

## How to contribute

### General 
***Note: all commands are executed from the project root***

Values for the environment variables mentioned in the docker-compose files have to be supplied. This can be done via .env file, replacing
the ${} with values directly or by setting environment values on the machine.

### Local Development
Run the app locally
> dotnet run

and only start the database via docker 
> docker-compose -f ./build/docker-compose.DOCKER.yml up real_ease_db --build --abort-on-container-exit --force-recreate --remove-orphans

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
> docker-compose -f ./build/docker-compose.DOCKER.yml up --build --abort-on-container-exit --force-recreate --remove-orphans

Before each PR, check if everything works with docker
> docker-compose -f ./build/docker-compose.CI.yml up --build --abort-on-container-exit --force-recreate --remove-orphans


### PROD
> docker-compose -f ./build/docker-compose.yml up --build --abort-on-container-exit --force-recreate --remove-orphans


### Troubleshooting
Build specific stage from the Dockerfile in /build
> docker build -t real_ease -f .\build\Dockerfile --target [targetName] .

Then use
> docker run -t -d real_ease

to start the container and keep it running so e.g. portainer can be used to look inside.

___
## TODOs
* It would be nice to use IQuery<TQueryResult>/ICommand<TCommandResult> so the generics do not have to be specified each time
  * MediatR does this but the Dispatchers seem to have to be way more complicated
    * see discussion (https://cezarypiatek.github.io/post/why-i-dont-use-mediatr-for-cqrs/)
  * rethink application wide errors https://enterprisecraftsmanship.com/posts/advanced-error-handling-techniques/
* Use a more functional approach in the Application services (Handlers), like csharpfunctionalextensions

