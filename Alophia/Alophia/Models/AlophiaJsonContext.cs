using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Alophia.Models;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = true)]
[JsonSerializable(typeof(AlophiaProject))]
[JsonSerializable(typeof(Stage))]
[JsonSerializable(typeof(Bot))]
[JsonSerializable(typeof(Requirement))]
[JsonSerializable(typeof(RequirementTask))]
[JsonSerializable(typeof(Repository))]
[JsonSerializable(typeof(Commit))]
[JsonSerializable(typeof(FileDiff))]
[JsonSerializable(typeof(DmMessage))]
[JsonSerializable(typeof(IReadOnlyDictionary<string, IReadOnlyList<DmMessage>>))]
public partial class AlophiaJsonContext : JsonSerializerContext
{
}
