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
            DiscordEmoji[] emojis = { DiscordEmoji.FromUnicode("🧡"), DiscordEmoji.FromUnicode("💚"), DiscordEmoji.FromUnicode("💙")};

            var msg = await ctx.RespondAsync($"Hey, {ctx.User.Mention}!\n" +
                "What platform do you play on ?\n" +
                "Respond with one of the following emojis\n" +
                ":orange_heart: - Origin  :green_heart: - Xbox  :blue_heart: - Playstation");

            for (var i = 0; i < emojis.Length; ++i)
            {
                await msg.CreateReactionAsync(emojis[i]);
                Thread.Sleep(500);
            }
                
        }
    }
}
