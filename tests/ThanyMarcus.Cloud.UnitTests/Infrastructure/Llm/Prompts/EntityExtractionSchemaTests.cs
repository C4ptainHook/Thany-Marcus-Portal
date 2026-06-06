using System.Text.Json.Nodes;
using Shouldly;
using ThanyMarcus.Cloud.Api.Features.Entities;
using ThanyMarcus.Cloud.Api.Infrastructure.Llm.Prompts;

namespace ThanyMarcus.Cloud.Tests.Infrastructure.Llm.Prompts;

public sealed class EntityExtractionSchemaTests
{
    private static readonly string[] ExpectedKinds =
    {
        EntityKind.Person, EntityKind.Organization,
        EntityKind.Place, EntityKind.Concept, EntityKind.Other,
    };

    private static readonly string[] ExpectedRequired =
    {
        "anchor_text", "start_offset", "end_offset",
        "candidate_kind", "candidate_canonical", "aliases", "confidence",
    };

    private static JsonObject MentionItem()
    {
        var schema = EntityExtractionSchema.Build();
        return (JsonObject)schema["properties"]!["mentions"]!["items"]!;
    }

    [Fact]
    public void Roots_an_object_with_a_required_mentions_array()
    {
        var schema = EntityExtractionSchema.Build();
        schema["type"]!.GetValue<string>().ShouldBe("object");
        schema["additionalProperties"]!.GetValue<bool>().ShouldBeFalse();
        ((JsonArray)schema["required"]!).Select(n => n!.GetValue<string>()).ShouldContain("mentions");
        schema["properties"]!["mentions"]!["type"]!.GetValue<string>().ShouldBe("array");
    }

    [Fact]
    public void Constrains_candidate_kind_to_the_entity_kind_enum()
    {
        var kinds = ((JsonArray)MentionItem()["properties"]!["candidate_kind"]!["enum"]!)
            .Select(n => n!.GetValue<string>())
            .ToArray();
        kinds.ShouldBe(ExpectedKinds, ignoreOrder: true);
    }

    [Fact]
    public void Caps_anchor_canonical_and_alias_lengths()
    {
        var props = MentionItem()["properties"]!;
        props["anchor_text"]!["maxLength"]!.GetValue<int>().ShouldBe(40);
        props["candidate_canonical"]!["maxLength"]!.GetValue<int>().ShouldBe(60);
        props["aliases"]!["items"]!["maxLength"]!.GetValue<int>().ShouldBe(40);
    }

    [Fact]
    public void Requires_every_mention_field()
    {
        var required = ((JsonArray)MentionItem()["required"]!)
            .Select(n => n!.GetValue<string>())
            .ToArray();
        required.ShouldBe(ExpectedRequired, ignoreOrder: true);
    }
}
