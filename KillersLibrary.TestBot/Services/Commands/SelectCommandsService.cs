using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Discord.WebSocket;
using Discord.Rest;
using Discord;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using KillersLibraryTestBot.Services;

namespace KillersLibraryTestBot.Services.Commands {
    public class SelectCommandsService {
        private readonly CommonFunctionService _commonFunctionService;

        public SelectCommandsService( UserService userService, CommonFunctionService commonFunctioService) {
            _commonFunctionService = commonFunctioService;
        }

        #region AddPersonToFamily
        public async Task ChooseParentType(SocketMessageComponent interaction, ulong value) {
            if (interaction.Message.Reference.MessageId.IsSpecified) {
                IMessage repliedIMessage = await interaction.Channel.GetMessageAsync(interaction.Message.Reference.MessageId.Value);

                await interaction.Message.DeleteAsync();
                await repliedIMessage.DeleteAsync();
                await interaction.Channel.SendMessageAsync(text: "Add person to Family:",
                    components: _commonFunctionService.RemakeMessage(repliedIMessage, "connectionType", ButtonStyle.Danger).Build());
            }
        }
        #endregion
    }
}
