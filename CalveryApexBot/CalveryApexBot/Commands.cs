using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;

namespace CalveryApexBot
{
    public class Commands
    {
        private RestClient client = new RestClient("https://public-api.tracker.gg/v2/apex/standard/profile/");
        private SqliteConnection connection = new SqliteConnection("Data Source=C:/Users/Keven/Desktop/CalveryApexBot/bot.db");
        
        [Command("rank")]
        public async Task Rank(CommandContext ctx)
        {
            await connection.OpenAsync();

            string platform, username;

            //Get the username and platform for the command caller
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                    SELECT apex_platform, apex_username
                    FROM users
                    WHERE discord_userId = $id
                 ";
            command.Parameters.AddWithValue("$id", ctx.User.Id);

            using (var reader = await command.ExecuteReaderAsync())
            {
                await reader.ReadAsync();

                platform = reader.GetString(0);
                username = reader.GetString(1);
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
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            //Transform data ranceived into better formated JSON object
            var userData = JObject.Parse(await tcs.Task);
            //Get the user rank score from the data received
            var userRank = userData["data"]["segments"][0]["stats"]["rankScore"]["metadata"]["rankName"].ToString();
            userRank = Regex.Replace(userRank, "[0-9]", "");
            userRank = userRank.Trim().ToUpper();

            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = 
                @"
                    UPDATE users
                    SET apex_rank = $apex_rank
                    WHERE discord_userId = $discord_userId
                 ";
            updateCommand.Parameters.AddWithValue("$apex_rank", userRank);
            updateCommand.Parameters.AddWithValue("$discord_userId", ctx.User.Id);

            await updateCommand.ExecuteNonQueryAsync();
            
            switch (userRank)
            {
                case "BRONZE":
                    await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(690322821078974504));
                    break;
                case "SILVER":
                    await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(690322860371476561));
                    break;
                case "GOLD":
                    await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(690322910920966305));
                    break;
                case "PLATINUM":
                    await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(690232097452982402));
                    break;
                case "DIAMOND":
                    await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(690322950188171344));
                    break;
                case "MASTER":
                    await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(690323080437956609));
                    break;
                default:
                    await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(690323104630964254));
                    break;
            }

            await ctx.RespondAsync($"{ctx.User.Mention} your rank has been updated !");

            await connection.CloseAsync();
        }

        [Command("verifyme")]
        public async Task Verify(CommandContext ctx, String username)
        {
            //Get Guild verified role
            DiscordRole verifiedRole = ctx.Guild.GetRole(690570494348492841);
            //Get member's roles
            var callerRoles = ctx.Member.Roles;
            //Verify if caller already has been verified
            foreach (DiscordRole role in callerRoles)
            {
                if (role == verifiedRole)
                {
                    await ctx.RespondAsync($"You have already been verified {ctx.User.Mention}!\n" + "Use the .rank command instead to update your current rank.");
                    return;
                }
            }

            await connection.OpenAsync();

            string platform = "";
            var interactivity = ctx.Client.GetInteractivityModule();
            //Create array of the three used emojis for selecting platforms
            DiscordEmoji[] emojis = { DiscordEmoji.FromUnicode("🧡"), DiscordEmoji.FromUnicode("💚"), DiscordEmoji.FromUnicode("💙")};

            var msg = await ctx.RespondAsync($"Hey, {ctx.User.Mention}!\n" +
                "What platform do you play on ?\n\n" +
                ":orange_heart: - Origin  :green_heart: - Xbox  :blue_heart: - Playstation");

            //Waits for a reaction on the sent message and, when getting one, verifies the emoji is part of the emojis array
            var userReaction = await interactivity.WaitForReactionAsync(xe => Array.Exists(emojis, x => x == xe), ctx.User, TimeSpan.FromMinutes(1));          
            
            if(userReaction != null)
            {
                switch(userReaction.Emoji.Name)
                {
                    case "🧡":
                        platform = "origin";
                        break;
                    case "💚":
                        platform = "xbl";
                        break;
                    case "💙":
                        platform = "psn";
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
            await ctx.RespondAsync($"What is your current rank score {ctx.User.Mention} ?");

            var userResponse = await interactivity.WaitForMessageAsync(msg => msg.Content.Contains(userRankScore.ToString()));

            var command = connection.CreateCommand();
            command.CommandText =
                @"
                    INSERT INTO users (discord_userId, apex_platform, apex_username, apex_rank) 
                    VALUES ($discord_userId, $apex_platform, $apex_username, $apex_rank); 
                 ";
            command.Parameters.AddWithValue("$discord_userId", ctx.Member.Id);
            command.Parameters.AddWithValue("$apex_platform", platform);
            command.Parameters.AddWithValue("$apex_username", username);
            command.Parameters.AddWithValue("$apex_rank", "");

            await command.ExecuteNonQueryAsync();

            if (userResponse != null)
            {
                await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(690570494348492841));
                await ctx.RespondAsync($"Perfect ! You are now verified {ctx.User.Mention}!");
            }

            await connection.CloseAsync();
        }
    }
}
