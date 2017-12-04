namespace Mint {
	using System;

	public abstract class Component {

		public string Name => GetType().Name;
		protected Entity ent;

		public Entity Entity {
			get => ent;
			protected internal set => ent = value;
		}

		public bool SetEnt(Entity nent) {
			if (nent != null) {
				try { nent.Add(this); }
				catch (ArgumentException) { return false; }
				return true;
			}
			Entity prevEnt = Entity;
			prevEnt.Rem(Name);
			return true;
		}

		public override string ToString() {
			return Name + " : " + Entity;
		}

		public virtual void OnAdded(Entity to) { }
		public virtual void OnRemoved(Entity from) { }
	}
}