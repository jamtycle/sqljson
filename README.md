# JSON to SQL

This is a library to transform a JSON to an SQL statement in different flavors using C#.

The goal is to have a structured way to build an SQL statement using JSON. The project have the philosophy to have a way to build SQL that can be sended from a client without the risk of having SQL injection in the way. To avoid this, spaces are not allowed in any of the fields, any space will result in an error. (Spaces are only allowed between single quotes).

## Milestones

- sqljson schema v1.
- sqljson C# core lib v1 (using postgresql flavor for QUERIES).
- sqljson C++ binaries to use outside dotnet framework.
- nested queries.
- sequential queries.
- temp tables support.
