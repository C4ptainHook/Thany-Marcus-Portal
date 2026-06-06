using System.Text.Json.Nodes;
using ThanyMarcus.Cloud.Api.Features.Entities;

namespace ThanyMarcus.Cloud.Api.Infrastructure.Llm.Prompts;

public static class EntityExtractionSchema
{
    private const int MaxAnchorChars = 40;
    private const int MaxCanonicalChars = 60;
    private const int MaxAliasChars = 40;
    private const int MaxMentions = 50;
    private const int MaxAliases = 6;

    public static JsonObject Build() => new()
    {
        ["type"] = "object",
        ["properties"] = new JsonObject
        {
            ["mentions"] = new JsonObject
            {
                ["type"] = "array",
                ["maxItems"] = MaxMentions,
                ["items"] = MentionSchema(),
            },
        },
        ["required"] = new JsonArray("mentions"),
        ["additionalProperties"] = false,
    };

    private static JsonObject MentionSchema() => new()
    {
        ["type"] = "object",
        ["properties"] = new JsonObject
        {
            ["anchor_text"] = StringSchema(MaxAnchorChars),
            ["start_offset"] = new JsonObject { ["type"] = "integer" },
            ["end_offset"] = new JsonObject { ["type"] = "integer" },
            ["candidate_kind"] = new JsonObject
            {
                ["type"] = "string",
                ["enum"] = new JsonArray(
                    EntityKind.Person, EntityKind.Organization,
                    EntityKind.Place, EntityKind.Concept, EntityKind.Other),
            },
            ["candidate_canonical"] = StringSchema(MaxCanonicalChars),
            ["aliases"] = new JsonObject
            {
                ["type"] = "array",
                ["maxItems"] = MaxAliases,
                ["items"] = StringSchema(MaxAliasChars),
            },
            ["confidence"] = new JsonObject { ["type"] = "number" },
        },
        ["required"] = new JsonArray(
            "anchor_text", "start_offset", "end_offset",
            "candidate_kind", "candidate_canonical", "aliases", "confidence"),
        ["additionalProperties"] = false,
    };

    private static JsonObject StringSchema(int maxChars) => new()
    {
        ["type"] = "string",
        ["maxLength"] = maxChars,
    };
}
