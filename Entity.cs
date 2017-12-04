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

		uint key;

		Pool pool;

		public Pool Pool {
			get => pool;
			protected internal set {
				if (pool != null) { throw new NotImplementedException(); }

				pool = value;
			}
		}

		public uint Key {
			get => key;
			protected internal set => key = value;
		}

		public Entity() { }

		public Entity(params Component[] comps) {
			foreach (Component comp in comps) {
				Add(comp);
			}
		}

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

		public Component Get(string compType) {
			return !components.ContainsKey(compType) ? null : components[compType];
		}

		public bool Has<T>() where T : Component {
			return Has(typeof(T).Name);
		}

		public bool Has(string compType) {
			return components.ContainsKey(compType);
		}

		public void Add(Component comp) {
			if (comp == null) { throw new ArgumentNullException("Can't add a null component to an entity."); }
			if (Has(comp.Name)) { throw new ArgumentException("Entity already contains component of type " + comp.Name); }
			components.Add(comp.Name, comp);
			Entity prevEnt = comp.Entity;
			comp.Entity = this;
			comp.OnRemoved(prevEnt);
			prevEnt?.RemovedComp?.Invoke(prevEnt, new CompEventArgs(prevEnt, comp));
			comp.OnAdded(this);
			AddedComp?.Invoke(this, new CompEventArgs(prevEnt, comp));
		}

		public T Rem<T>() where T : Component {
			T comp = Get<T>();
			if (comp == null) { throw new ArgumentNullException("Entity does not contain a component of that type."); }
			components.Remove(comp.Name);
			comp.Entity = null;
			comp.OnRemoved(this);
			RemovedComp?.Invoke(this, new CompEventArgs(this, comp));
			return comp;
		}

		public Component Rem(string compType) {
			Component comp = Get(compType);
			if (comp == null) { throw new ArgumentNullException("Entity does not contain component of that type."); }
			components.Remove(comp.Name);
			comp.Entity = null;
			comp.OnRemoved(this);
			RemovedComp?.Invoke(this, new CompEventArgs(this, comp));
			return comp;
		}

	}
}