using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System;
using Discord;

namespace Interaction.Services {
    //Extensions related to components.
    public static class ComponentExtension {
        //Get the component that matches the customId.
        public static IMessageComponent GetComponent(this IReadOnlyCollection<ActionRowComponent> rows, string customId) {
            //Go through the rows.
            foreach (var row in rows) {
                //For each component in the row.
                foreach (var component in row.Components) {
                    //If the component is a button or menu and matches the id, return it.
                    switch (component) {
                        case SelectMenuComponent menu:
                            if (menu.CustomId == customId) return menu;
                            break;
                        case ButtonComponent button:
                            if (button.CustomId == customId) return button;
                            break;
                    }
                }
            }

            return null;
        }

        //Get the button that matches the customId.
        public static ButtonComponent GetButton(this IReadOnlyCollection<ActionRowComponent> rows, string customId)
            => rows.GetComponent(customId) as ButtonComponent;

        //Get the menu that matches the customId.
        public static SelectMenuComponent GetMenu(this IReadOnlyCollection<ActionRowComponent> rows, string customId)
            => rows.GetComponent(customId) as SelectMenuComponent;

        //Gets the option in a select menu that match the value.
        public static SelectMenuOption GetSelectedOption(this SelectMenuComponent menu, string value)
            => menu.Options.FirstOrDefault(x => x.Value == value);

        //Gets the options in a select menu that match the values.
        public static IReadOnlyCollection<SelectMenuOption> GetSelectedOptions(this SelectMenuComponent menu, IReadOnlyCollection<string> values)
            => values.Select(value => GetSelectedOption(menu, value)).ToImmutableList();

        //Get the component that matches the customId.
        public static IMessageComponent GetComponent(this ComponentBuilder builder, string customId) {
            //Go through the rows.
            foreach (var row in builder.ActionRows) {
                //For each component in the row.
                foreach (var component in row.Components) {

                    //If the component is a button or menu and matches the id, return it.
                    switch (component) {
                        case SelectMenuComponent menu:
                            if (menu.CustomId == customId) return menu;
                            break;
                        case ButtonComponent button:
                            if (button.CustomId == customId) return button;
                            break;
                    }
                }
            }

            return null;
        }

        //Update the button with the given id.
        public static bool UpdateButton(this ComponentBuilder component, string customId, Action<ButtonBuilder> func) {
            foreach (var row in component.ActionRows) {
                if (row.UpdateButton(customId, func)) return true;
            }

            return false;
        }

        //Update the button with the given id.
        public static bool UpdateButton(this ActionRowBuilder row, string customId, Action<ButtonBuilder> func) {
            //Find the button with the matching id.
            var components = row.Components;
            for (int i = 0; i < components.Count; ++i) {
                if (components[i] is ButtonComponent button && button.CustomId == customId) {
                    var builder = button.ToBuilder();
                    func(builder);
                    components[i] = builder.Build();
                    return true;
                }
            }

            return false;
        }

        //Disable all components in a message.
        public static ComponentBuilder DisableComponents(this ComponentBuilder component) {
            foreach (var row in component.ActionRows) {
                row.DisableComponents();
            }

            return component;
        }

        //Disable all components in a row.
        public static void DisableComponents(this ActionRowBuilder row) {
            //Find the button with the matching id.
            var components = row.Components;
            for (int i = 0; i < components.Count; ++i) {
                if (components[i] is ButtonComponent button) {
                    var builder = button.ToBuilder();
                    builder.Disabled = true;
                    components[i] = builder.Build();
                } else if (components[i] is SelectMenuComponent menu) {
                    var builder = menu.ToBuilder();
                    builder.Disabled = true;
                    components[i] = builder.Build();
                }
            }
        }

        #region Methods to convert to builders
        //Get a builder for a button.
        public static ButtonBuilder ToBuilder(this ButtonComponent button) {
            //var builder = ButtonBuilder.CreateSecondaryButton(button.Label, button.CustomId)
            //	.WithDisabled(button.Disabled)
            //	.WithEmote(button.Emote)
            //	.WithStyle(button.Style)
            //	.WithUrl(button.Url)
            //	;
            //
            //return builder;
            return new ButtonBuilder {
                Disabled = button.Disabled,
                Emote = button.Emote,
                Style = button.Style,
                CustomId = button.CustomId,
                Label = button.Label,
                Url = button.Url,
            };
        }

        //Get a builder for a menu.
        public static SelectMenuBuilder ToBuilder(this SelectMenuComponent menu) {
            return new SelectMenuBuilder {
                CustomId = menu.CustomId,
                //Label = menu.Label,
                Placeholder = menu.Placeholder,
                MinValues = menu.MinValues,
                MaxValues = menu.MaxValues,
                Options = menu.Options.Select(option => option.ToBuilder()).ToList(),
            };
        }

        //Get a builder for a menu option.
        public static SelectMenuOptionBuilder ToBuilder(this SelectMenuOption option) {
            return new SelectMenuOptionBuilder {
                Label = option.Label,
                Value = option.Value,
                Description = option.Description,
                Emote = option.Emote,
                Default = option.Default,
            };
        }

        //Get a builder for an action row.
        public static ActionRowBuilder ToBuilder(this ActionRowComponent row) {
            return new ActionRowBuilder {
                Components = row.Components.ToList(),
            };
        }

        //Get a builder for a MessageComponent.
        public static ComponentBuilder ToBuilder(this MessageComponent comp) {
            return new ComponentBuilder {
                ActionRows = comp.Components.Select(row => row.ToBuilder()).ToList(),
            };
        }

        //Get a ComponentBuilder from a list of rows.
        public static ComponentBuilder ToBuilder(this IReadOnlyCollection<IMessageComponent> rows) {
            var rowBuilders = rows
                .Select(row => row as ActionRowComponent)
                .Where(row => row != null)
                .Select(row => row.ToBuilder())
                .ToList();

            return new ComponentBuilder {
                ActionRows = rowBuilders,
            };
        }
        #endregion

        #region Methods for safely adding components to a builder
        delegate bool RowValidator(ActionRowBuilder row);

        //If the requested row is clear. (Not yet created, or has no components).
        public static bool IsClearRow(this ComponentBuilder builder, int row = 0) {
            //If there are no rows.
            if (builder.ActionRows == null) return true;
            //If the row isn't created.
            if (builder.ActionRows.Count <= row) return true;
            //If the row is empty.
            if (builder.ActionRows[row].Components.Count == 0) return true;
            return false;
        }

        //Finds the first row that is valid for our SelectMenuBuilder.
        public static int FindValidMenuRow(this ComponentBuilder builder, int rowNr = 0) {
            RowValidator rowValidator = row => row.CanAddMenu();

            return builder.FindValidRow(rowValidator, rowNr);
        }

        //Finds the first row that is valid for our ButtonBuilder.
        public static int FindValidButtonRow(this ComponentBuilder builder, int rowNr = 0) {
            RowValidator rowValidator = row => row.CanAddButton();

            return builder.FindValidRow(rowValidator, rowNr);
        }

        //Finds the first valid row index, or -1.
        private static int FindValidRow(this ComponentBuilder builder, RowValidator rowValidator, int rowNr) {
            //Must be a valid row number.
            if (rowNr < 0 || rowNr >= ComponentBuilder.MaxActionRowCount)
                throw new ArgumentOutOfRangeException(nameof(rowNr), rowNr, $"Must be in range [{0}, {ComponentBuilder.MaxActionRowCount})!");

            //The rows of the builder.
            var rows = builder?.ActionRows ?? new List<ActionRowBuilder>();

            //If the row is clear, it's valid.
            if (builder.IsClearRow(rowNr)) return rowNr;

            //Otherwise cycle through and find the first valid row.
            for (int i = rowNr; i < ComponentBuilder.MaxActionRowCount; ++i) {
                if (i >= rows.Count || rowValidator(rows?[i])) return i;
            }

            for (int i = 0; i < rowNr; ++i) {
                if (rowValidator(rows?[i])) return i;
            }

            //If no row is valid, return -1.
            return -1;
        }

        //Indicates if the row can have a SelectMenuBuilder added to it.
        public static bool CanAddMenu(this ActionRowBuilder builder) {
            //If it's empty, it's valid.
            if ((builder?.Components?.Count ?? 0) == 0) return true;
            //Otherwise, it's not valid for menu.
            return false;
        }

        //Indicates if the row can have a ButtonBuilder added to it.
        public static bool CanAddButton(this ActionRowBuilder builder) {
            //Number of children in this row.
            int childCount = builder?.Components?.Count ?? 0;

            //If it's full, it's not valid.
            if (childCount == ActionRowBuilder.MaxChildCount) return false;

            //If the row contains a SelectMenuBuilder, we can't have buttons on it.
            if (builder?.Components?.FirstOrDefault() is SelectMenuBuilder) return false;

            //Otherwise, it's not valid for menu.
            return true;
        }
        #endregion
    }
}