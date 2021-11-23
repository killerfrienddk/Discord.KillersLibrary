using System.Collections.Generic;
using Discord;

namespace KillersLibraryTestBot.Services {
    public class CommonFunctionService {
        public ButtonBuilder MakeGoBackButton(string label = "Go Back", ButtonStyle buttonStyle = ButtonStyle.Danger) {
            return new ButtonBuilder()
                .WithCustomId("goBack")
                .WithLabel(label)
                .WithStyle(buttonStyle);
        }

        public ComponentBuilder RemakeMessage(IMessage message, string buttonID, ButtonStyle style) {
            var builder = new ComponentBuilder();

            List<ButtonComponent> buttonComponents = new();
            foreach (ActionRowComponent comp in message.Components) {
                foreach (IMessageComponent messageComp in comp.Components) {
                    buttonComponents.Add((ButtonComponent)messageComp);
                }
            }

            bool allIsReady = true;
            int counter = 1;
            foreach (ButtonComponent buttonComponent in buttonComponents) {
                ButtonBuilder button = new();
                button.WithCustomId(buttonComponent.CustomId);
                button.WithLabel(buttonComponent.Label);

                if (buttonID == buttonComponent.CustomId) button.WithStyle(style);
                else button.WithStyle(buttonComponent.Style);

                if (button.Style == ButtonStyle.Secondary) {
                    builder.WithButton(button);
                    counter++;
                    continue;
                }

                if (!button.IsDisabled && allIsReady) allIsReady = button.Style == ButtonStyle.Danger;
                if (counter == buttonComponents.Count) {
                    button.WithDisabled(!allIsReady);
                    if (allIsReady) button.WithStyle(ButtonStyle.Success);
                }

                builder.WithButton(button);
                counter++;
            }

            return builder;
        }
    }
}
