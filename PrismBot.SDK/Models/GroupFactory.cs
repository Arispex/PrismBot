namespace PrismBot.SDK.Models;

public class GroupFactory
{
    public static Group CreateGroup(string groupName, Group? parent)
    {
        return new Group
        {
            GroupName = groupName,
            Parent = parent,
            Permissions = string.Empty
        };
    }
}