using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;

namespace CalveryApexBot
{
    class Program
    {
        static DiscordClient discord;
        static CommandsNextModule commands;
        static InteractivityModule interactivity;

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            discord = new DiscordClient(new DiscordConfiguration
            {
                //Searches the Windows Environement Variables for the token
                Token = Environment.GetEnvironmentVariable("token", EnvironmentVariableTarget.Machine),
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "."
            });;

            commands.RegisterCommands<Commands>();
            interactivity = discord.UseInteractivity(new InteractivityConfiguration
            {
                // default pagination behaviour to just ignore the reactions
                PaginationBehaviour = TimeoutBehaviour.Ignore,

                // default pagination timeout to 5 minutes
                PaginationTimeout = TimeSpan.FromMinutes(5),

                // default timeout for other actions to 2 minutes
                Timeout = TimeSpan.FromMinutes(2)
            });

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
