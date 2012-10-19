# BookKeeper

This is a work in progress!

This library aims to solve the problem of managing a block of state
representing an object, within a distributed system, where nodes may
partition.

Think of this as a potential solution to handling data in a system like
Riak.

## The Basics - What Does This Thing Do?

The API is designed to be as simple to consume as possible, with the
ability to serialize an object and it's state changes over time.

For instance, assume that an objects original state is

```csharp
var foo = new() { a = 1, b = 2, c = 3 };
```

Assume application code has modified the state of this object as such

```csharp
foo.b = 4;
```

BookKeeper will, upon serialization, persist a JSON structure like this

```javascript
{ "a" : 1, "b" : 2, "c" : 3, "diffs" : [ { "u": ["b" : 4], "t": "2012-10-16T06:19Z" } ] }
```

The idea is to start with a base object state, and to provide a list of
object state changes that is easy to merge into a single source of
truth on read repair, should a distributed cluster partition, then rejoin.

The list of diffs is relatively ordered by wall clock time and persisted
in [ISO 8601][ISO8601].

Note that this does require a level of clock synchronization across a
cluster of nodes to preserve *relative* ordering, which is reasonably
straightforward on EC2.  We feel that most systems will be OK with this
level of precicion, but all applications are different and this won't
work for everyone.

A side-effect is that all changes to an object can be retrieved at any time, making this library great for implementing an audit log.

[ISO8601]: http://en.wikipedia.org/wiki/ISO_8601

### Some Specifics


We aim to provide the tooling to

* Provide a generic wrapper around a given type `T`, so that callers are
unaware of the changeset log and it's persistence details - note that it
will be necessary for the persistence layer to know about the type, but
callers to a repository layer need not know these underlying details
* Supported nested object hierarchies of arbitrary depth
* Provide ways to serialize both update and deletes of entire nested
objects or items within collections
* Blazing fast replays of changeset history on a given object (after all
 this is mostly a series of property sets / collection modifications)
* Blazing fast serialization / deserialization of nested objects
* Provide a simple API for merging a set of divergent objects during a
read-repair to generate a single source of truth (using the relative
ordering described above).
* Expose the series of object changes to the consumer.


### The Garbage / Compaction Issue

You may be asking, what happens when I accumulate a large list of
changesets?  Won't replaying a list of changests each time an object is
read incur a performance penalty on each read?

Distributed data is an emerging area, and new techniques are being
developed all the time to create eventually consistent systems.  We feel
that using a simple / fast serialization method for changes, and using
fast techniques within a compiled language to set properties while
replaying changes is a fast enough approach.

The issue of compacting objects over time is not one that we will be
addressing at this time.  This is something that we are punting to a
higher application level at the moment, but could eventually implement.

There may be advantages to keeping an audit history of an object over
time, for instance.

If you have an object that is frequently updated in a data store, and
generates, for instance, thousands of changes over time, this might
not be the best implementation for you at this time.

## Inspiration

There were a lot of great talks at [Ricon2012][Ricon2012], and there has
been a lot of great research around [CRDT][CRDT] (Convergent and Commutative
Replicated Data Types).

Our goal is to bring something easy to understand and use into the fold.

[Ricon2012]: http://basho.com/community/ricon2012/
[CRDT]: http://hal.inria.fr/docs/00/60/93/99/PDF/RR-7687.pdf

## Libraries Involved

* [EqualityComparer][EqualityComparer] - Is capable of comparing two
objects of the same type quickly using a cached Expression tree.  The
diff between current object state and prior object state is calculated.

* [FastMember][FastMember] - Is used to quickly replay the changesets
to the base immutable object.

* [ServiceStack.Text][ServiceStack.Text] - Provides serialization support
for nested object graphs.

[EqualityComparer]: https://github.com/EastPoint/EqualityComparer
[FastMember]: http://code.google.com/p/fast-member/
[ServiceStack.Text]: https://github.com/ServiceStack/ServiceStack.Text
