using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;

namespace CalveryApexBot
{
    public class Commands
    {
        private RestClient client = new RestClient("https://public-api.tracker.gg/v2/apex/standard/profile/");
        

        [Command("verifyme")]
        public async Task Verify(CommandContext ctx, String username)
        {
            string platform = "";
            var interactivity = ctx.Client.GetInteractivityModule();
            //Create array of the three used emojis for selecting platforms
            DiscordEmoji[] emojis = { DiscordEmoji.FromUnicode("🧡"), DiscordEmoji.FromUnicode("💚"), DiscordEmoji.FromUnicode("💙")};

            var msg = await ctx.RespondAsync($"Hey, {ctx.User.Mention}!\n" +
                "What platform do you play on ?\n\n" +
                ":orange_heart: - Origin  :green_heart: - Xbox  :blue_heart: - Playstation");

            for (var i = 0; i < emojis.Length; ++i)
            {
                //Adds each emoji as reactions to previously sent message
                await msg.CreateReactionAsync(emojis[i]);
                //Makes thread sleep to not hit limit of 
                //Discord's API of 2 actions per second
                Thread.Sleep(100);
            }

            //Waits for a reaction on the sent message and, when getting one, verifies the emoji is part of the emojis array
            var userReaction = await interactivity.WaitForReactionAsync(xe => Array.Exists(emojis, x => x == xe), ctx.User, TimeSpan.FromMinutes(1));          
            
            if(userReaction != null)
            {
                switch(userReaction.Emoji.Name)
                {
                    case "🧡":
                        platform = "origin";
                        await ctx.RespondAsync($"Origin {username}");
                        break;
                    case "💚":
                        platform = "xbl";
                        await ctx.RespondAsync($"Xbox {username}");
                        break;
                    case "💙":
                        platform = "psn";
                        await ctx.RespondAsync($"Playstation {username}");
                        break;
                };
            } else
            {
                await ctx.RespondAsync("didn't work");
            }

            var tcs = new TaskCompletionSource<string>();

            try
            {
                //Creates request to execute later
                RestRequest request = new RestRequest($"{platform}/{username}", Method.GET);
                request.AddHeader("TRN-Api-Key", Environment.GetEnvironmentVariable("TRN-Api-Key"));
                request.RequestFormat = DataFormat.Json;

                client.GetAsync(request, (response, handle) =>
                {
                    if ((int)response.StatusCode >= 400)
                    {
                        tcs.SetException(new Exception(response.StatusDescription));
                    }
                    else
                    {
                        tcs.SetResult(response.Content);
                    }

                });
            } catch(Exception ex)
            {
                tcs.SetException(ex);
            }

            //Transform data ranceived into better formated JSON object
            var userData = JObject.Parse(await tcs.Task); 
            //Get the user rank score from the data received
            var userRankScore = userData["data"]["segments"][0]["stats"]["rankScore"]["value"];
            Console.WriteLine(userRankScore);

            await ctx.RespondAsync("What is your current rank score ?");

            var userResponse = await interactivity.WaitForMessageAsync(msg => msg.Content.Contains(userRankScore.ToString()));

            if(userResponse != null)
            {
                await ctx.Guild.GrantRoleAsync(ctx.Member, ctx.Guild.GetRole(690570494348492841));
                await ctx.RespondAsync("Perfect ! You are now verified !");
            }
        }
    }
}
