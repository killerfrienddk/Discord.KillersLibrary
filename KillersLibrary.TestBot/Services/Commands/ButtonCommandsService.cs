using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Rest;
using Discord;
using KillersLibrary.Services;

namespace KillersLibraryTestBot.Services.Commands {
    public class ButtonCommandsService {
        private readonly CommonFunctionService _commonFunctionService;
        private readonly MultiButtonsService _multiButtonsService;
        private readonly UserService _userService;

        public ButtonCommandsService(CommonFunctionService commonFunctionService, MultiButtonsService multiButtonsService, UserService userService) {
            _commonFunctionService = commonFunctionService;
            _multiButtonsService = multiButtonsService;
            _userService = userService;
        }

        public async Task CreateChooseChildButtons(SocketMessageComponent interaction) {
            var user = (SocketGuildUser)interaction.User;
            List<RestGuildUser> users = await _userService.GetSortedUserListAsync(user.Guild);
            List<string> userList = new();
            users.ForEach(u => userList.Add(u.Nickname ?? u.Username));
            ComponentBuilder builder = _multiButtonsService.CreateMultiButtons(userList, new() { CustomID = "multiButtons" });

            builder.WithButton(_commonFunctionService.MakeGoBackButton());

            await interaction.FollowupAsync(text: "Choose Person", components: builder.Build());
        }

        public async Task ChooseChildNameRange(SocketMessageComponent interaction) {
            List<RestGuildUser> users = await _userService.GetSortedUserListAsync(((SocketGuildUser)interaction.User).Guild);

            List<MultiButton> multibuttons = new();
            foreach (RestGuildUser user in users) {
                MultiButton multiButton = new() {
                    Title = user.Nickname ?? user.Username,
                    Value = user.Id.ToString()
                };

                multibuttons.Add(multiButton);
            }

            var builder = _multiButtonsService.CreateSelectForMultiButtons(interaction, multibuttons, new() { CustomID = "selectChild" });

            await interaction.FollowupAsync("Choose Person", components: builder.Build());
        }
    }
}