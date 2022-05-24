﻿using Application.UseCases.Base;
using Application.UseCases.Website;
using Application.UseCases.Website.QueryAvailableWebsites;
using Discord.Commands;

namespace Infrastructure.Discord.Commands;

public class ListWebsites : ModuleBase<SocketCommandContext>
{
    private readonly IQueryDispatcher _queryDispatcher;

    public ListWebsites(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [Command("listWebsites")]
    [Alias("lW")]
    public async Task ListWebsitesHandler()
    {
        var availableWebsites =
            await _queryDispatcher.Dispatch<AvailableWebsitesQuery, AvailableWebsites>(new AvailableWebsitesQuery());

        var message = availableWebsites.Websites.Any()
            ? "Available Websites:" +
              availableWebsites.Websites.Aggregate("", (current, availableWebsite) => current + $"\n{availableWebsite.Name} Base URL: {availableWebsite.Url}")
            : "\nNo websites available.";

        await Context.Message.Channel.SendMessageAsync(message);
    }
}