using System.Collections.Generic;
using Discord;



namespace Interaction.TestingGrounds.Parameter {
	public class MenuSelectionParams : MessageComponentParams {
		
		private SelectMenuComponent _component;
		public new SelectMenuComponent Component { 
			get => _component; 
			set {
				_component = value;
				base.Component = value;
			} 
		}

		public IReadOnlyCollection<SelectMenuOption> Options { get; set; }
	}
}