using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

namespace CalveryApexBot
{
    public class Commands
    {
        [Command("verifyme")]
        public async Task Verify(CommandContext ctx)
        {
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
                Thread.Sleep(500);
            }
                
        }
    }
}
