# Mint
Entity-Component System framework, originally made for use in Garden and Descent.

## Manual:

### Clone & Build:
  
This uses [Walker](https://github.com/Music-Theory/Walker) as a submodule, so you'll need to clone this recursively. (If you don't, git will just make an empty folder for Walker and won't actually download it. You can fix that with `git submodule update`)
  
  git clone --recursive https://github.com/Music-Theory/Mint.git 
  cd Mint
  dotnet build
    
### Usage:
  
Entities are contained in pools. To create an entity and add it to a pool:

  Pool example = new Pool();
  Entity ent = new Entity();
  example.Add(ent);

When an entity is added to a pool, the pool assigns it an ID that can be used to reference the entity. To do that:

  uint key = ent.Key;
  Entity entRef = example[key];
  
You can also remove an entity from a pool using `pool.Rem(uint key)` or `pool.Rem(Entity ent)`. An entity not contained in a pool has its key = 0.

A component is an object defining some data that can be held by an entity. All components inherit from Component. To add a component to an Entity, assuming we've made a subclass of Component called "CompExample," use `ent.Add(new CompExample())`.
  
An entity can only hold one component of each type. So, it can't have two CompExamples.

To reference a component held by an entity, use `comp.Get<T>()` or `comp.Get("T")` where T is the type of the component. To remove a component, use `comp.Rem<T>()` or `comp.Rem("T")`.

there's also some other stuff but i don't feel like writing more of this
