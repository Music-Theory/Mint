namespace Mint {
	using System;

	public abstract class Component {

		public string Name => GetType().Name;
		protected Entity ent;

		public Entity Entity {
			get => ent;
			protected internal set => ent = value;
		}

		public override string ToString() {
			return Name + " : " + Entity;
		}

	}
}