using Discord;

namespace Interaction.TestingGrounds.Parameter {
	public class ButtonClickParams : MessageComponentParams {
		private ButtonComponent _component;
		public new ButtonComponent Component { 
			get => _component; 
			set {
				_component = value;
				base.Component = value;
			} 
		}
	}
}