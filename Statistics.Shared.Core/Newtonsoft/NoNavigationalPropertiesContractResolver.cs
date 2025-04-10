using System.Collections;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using Statistics.Shared.Abstraction.Interfaces.Models.Searchable;

namespace Statistics.Shared.Core.Newtonsoft;

public class NoNavigationalPropertiesContractResolver : CamelCasePropertyNamesContractResolver
{
    /// <inheritdoc />
    protected override List<MemberInfo> GetSerializableMembers(Type objectType)
    {
        // If the object type implements IComplexSearchable, return the members without filtering
        if (typeof(IComplexSearchable).IsAssignableFrom(objectType))
        {
            return base.GetSerializableMembers(objectType);
        }

        var members = base.GetSerializableMembers(objectType);

        // Filter out navigational properties
        members = members.Where(m => !IsNavigationalProperty(m)).ToList();

        return members;
    }

    private bool IsNavigationalProperty(MemberInfo member)
    {
        Type? memberType = GetMemberUnderlyingType(member);

        // Check if the member type is a collection or a reference to another entity
        return (typeof(IEnumerable).IsAssignableFrom(memberType) && memberType != typeof(string)) ||
               (memberType.IsClass && memberType != typeof(string));
    }

    private Type GetMemberUnderlyingType(MemberInfo member)
    {
        return member switch
        {
            FieldInfo field => field.FieldType,
            PropertyInfo property => property.PropertyType,
            var _ => throw new ArgumentException("MemberInfo must be of type FieldInfo or PropertyInfo",
                nameof(member)),
        };
    }
}
