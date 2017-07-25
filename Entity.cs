namespace Mint {
	using System;
	using System.Collections.Generic;

	public class CompEventArgs : EventArgs {

		public Entity prevEnt;
		public Component comp;

		public CompEventArgs(Entity prevEnt, Component comp) {
			this.prevEnt = prevEnt;
			this.comp = comp;
		}
	}

	public class Entity {

		public event EventHandler<CompEventArgs> AddedComp;
		public event EventHandler<CompEventArgs> RemovedComp;

		Dictionary<string, Component> components = new Dictionary<string, Component>();

		/// <summary>
		/// Gets component of specified type contained by this entity. Returns null if no such component.
		/// </summary>
		/// <typeparam name="T">Type of component to get.</typeparam>
		/// <returns>Component of specified type</returns>
		public T Get<T>() where T : Component {
			string name = typeof(T).Name;
			if (!components.ContainsKey(name)) { return null; }
			return components[name] as T;
		}

		public bool Contains<T>() where T : Component {
			return Contains(typeof(T).Name);
		}

		public bool Contains(string compType) {
			return components.ContainsKey(compType);
		}

		public void Add(Component comp) {
			if (comp == null) { throw new ArgumentException("Tried to add a null component to an entity."); }
			if (Contains(comp.Name)) { throw new ArgumentException("Entity already contains component of type " + comp.Name); }
			components.Add(comp.Name, comp);
			Entity prevEnt = comp.Entity;
			comp.Entity = this;
			AddedComp?.Invoke(this, new CompEventArgs(prevEnt, comp));
		}

		public T Rem<T>() where T : Component {
			if (!Contains<T>()) { throw new ArgumentException("Entity does not contain a component of that type."); }
			T comp = Get<T>();
			components.Remove(comp.Name);
			comp.Entity = null;
			RemovedComp?.Invoke(this, new CompEventArgs(this, comp));
			return comp;
		}

	}
}