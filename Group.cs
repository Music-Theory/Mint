namespace Mint {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	public class Group : IEnumerable<Entity> {

		public event EventHandler<EntEventArgs> EntAdded;
		public event EventHandler<EntEventArgs> EntRemoved;

		public Dictionary<Pool, List<uint>> keys = new Dictionary<Pool, List<uint>>();

		public List<string> compTypes;

		public List<Entity> this[Pool pool] => keys[pool].Select(key => pool[key]).ToList();

		public Group(IEnumerable<string> compTypes, params Pool[] pools) {
			this.compTypes = compTypes.ToList();
			foreach (Pool pool in pools) {
				Add(pool);
			}
		}

		public Group(IEnumerable<Type> compTypes, params Pool[] pools) {
			this.compTypes = new List<string>();
			foreach (Type type in compTypes) {
				this.compTypes.Add(type.Name);
			}
			foreach (Pool pool in pools) {
				Add(pool);
			}
		}

		public void Add<T>() where T : Component {
			string type = typeof(T).Name;
			if (compTypes.Contains(type)) { throw new ArgumentException("Already watching given comp type."); }
			compTypes.Add(type);
			RefreshAll();
		}

		public bool Rem<T>() where T : Component {
			string type = typeof(T).Name;
			if (compTypes.Remove(type)) {
				RefreshAll();
				return true;
			}
			return false;
		}

		public void Add(Pool pool) {
			if (keys.ContainsKey(pool)) { throw new ArgumentException("Already watching given pool."); }
			keys.Add(pool, new List<uint>());
			Sub(pool);
			Refresh(pool);
		}

		public bool Rem(Pool pool) {
			if (EntRemoved != null) { throw new NotImplementedException(); }
			if (keys.Remove(pool)) {
				Unsub(pool);
				return true;
			}
			return false;
		}

		void Add(Entity ent, Pool pool) {
			keys[pool].Add(ent.Key);
			EntAdded?.Invoke(this, new EntEventArgs(pool, ent.Key, ent));
		}

		public void Refresh(Pool pool) {
			foreach (uint key in keys[pool]) {
				EntRemoved?.Invoke(this, new EntEventArgs(pool, key, pool[key]));
			}
			keys[pool].Clear();
			foreach (Entity entity in pool) {
				if (IsValid(entity)) {
					Add(entity, pool);
				}
			}
		}

		public void RefreshAll() {
			foreach (KeyValuePair<Pool,List<uint>> entry in keys) {
				Refresh(entry.Key);
			}
		}

		bool IsValid(Entity ent) {
			return compTypes.All(ent.Has);
		}

		void Sub(Pool pool) {
			pool.AddedEnt += OnPoolAddedEnt;
			pool.RemovedEnt += OnPoolRemovedEnt;
			pool.EntAddedComp += OnEntAddedComp;
			pool.EntRemovedComp += OnEntRemovedComp;
		}

		void Unsub(Pool pool) {
			pool.AddedEnt -= OnPoolAddedEnt;
			pool.RemovedEnt -= OnPoolRemovedEnt;
			pool.EntAddedComp -= OnEntAddedComp;
			pool.EntRemovedComp -= OnEntRemovedComp;
		}

		void OnPoolAddedEnt(object sender, EntEventArgs entEventArgs) {
			Entity ent = entEventArgs.ent;
			if (IsValid(ent)) { Add(ent, ent.Pool); }
		}

		void OnPoolRemovedEnt(object sender, EntEventArgs entEventArgs) {
			keys[entEventArgs.prevPool].Remove(entEventArgs.prevKey);
			EntRemoved?.Invoke(this, entEventArgs);
		}

		void OnEntAddedComp(object sender, CompEventArgs e) {
			if (!compTypes.Contains(e.comp.Name)) { return; }
			Entity ent = e.comp.Entity;
			if (IsValid(ent)) {
				Add(ent, ent.Pool);
			}
		}

		void OnEntRemovedComp(object sender, CompEventArgs e) {
			if (!compTypes.Contains(e.comp.Name)) { return; }
			Entity prevEnt = e.prevEnt;
			if (keys[prevEnt.Pool].Contains(prevEnt.Key)) {
				keys[prevEnt.Pool].Remove(prevEnt.Key);
				EntRemoved?.Invoke(this, new EntEventArgs(prevEnt.Pool, prevEnt.Key, prevEnt));
			}
		}

		public IEnumerator<Entity> GetEnumerator() {
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}