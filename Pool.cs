namespace Mint {
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class EntEventArgs : EventArgs {

		public Pool prevPool;
		public uint prevKey;
		public Entity ent;

		public EntEventArgs(Pool prevPool, uint prevKey, Entity ent) {
			this.prevPool = prevPool;
			this.prevKey = prevKey;
			this.ent = ent;
		}
	}

	public class Pool : IEnumerable<Entity> {

		public event EventHandler<EntEventArgs> AddedEnt;
		public event EventHandler<EntEventArgs> RemovedEnt;
		public event EventHandler<CompEventArgs> EntAddedComp;
		public event EventHandler<CompEventArgs> EntRemovedComp;

		uint cKey = 1;

		Dictionary<uint, Entity> entities = new Dictionary<uint, Entity>();

		public Entity this[uint key] {
			get => Contains(key) ? entities[key] : null;
		}

		public void Add(Entity ent) {
			if (ent == null) { throw new ArgumentNullException("Can't add a null entity to a pool."); }
			Pool prevPool = ent.Pool;
			uint prevKey = ent.Key;
			prevPool?.Rem(ent, true);
			ent.Key = cKey++;
			entities[ent.Key] = ent;
			ent.Pool = this;
			entities.Add(ent.Key, ent);
			Sub(ent);
			prevPool?.Unsub(ent);
			prevPool?.RemovedEnt?.Invoke(prevPool, new EntEventArgs(prevPool, prevKey, ent));
			AddedEnt?.Invoke(this, new EntEventArgs(prevPool, prevKey, ent));
		}

		void Sub(Entity ent) {
			ent.AddedComp += OnEntAddedComp;
			ent.RemovedComp += OnEntRemovedComp;
		}

		void Unsub(Entity ent) {
			ent.AddedComp -= OnEntAddedComp;
			ent.RemovedComp -= OnEntRemovedComp;
		}

		void OnEntAddedComp(object sender, CompEventArgs compEventArgs) {
			EntAddedComp?.Invoke(sender, compEventArgs);
		}

		void OnEntRemovedComp(object sender, CompEventArgs compEventArgs) {
			EntRemovedComp?.Invoke(sender, compEventArgs);
		}

		public bool Contains(Entity ent) {
			return Contains(ent.Key);
		}

		public bool Contains(uint key) {
			return entities.ContainsKey(key);
		}

		// ReSharper disable once MethodOverloadWithOptionalParameter
		void Rem(Entity ent, bool silent = false) {
			if (!Contains(ent)) { throw new ArgumentOutOfRangeException("Entity not in this pool."); }
			entities.Remove(ent.Key);
			ent.Pool = null;
			uint prevKey = ent.Key;
			ent.Key = 0;
			Unsub(ent);
			if (!silent) { RemovedEnt?.Invoke(this, new EntEventArgs(this, prevKey, ent)); }
		}

		public void Rem(Entity ent) {
			Rem(ent, false);
		}

		public void Rem(uint key) {
			if (!Contains(key)) {throw new ArgumentOutOfRangeException("Entity not in this pool.");}
			Rem(entities[key]);
		}

		public IEnumerator<Entity> GetEnumerator() {
			return entities.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}