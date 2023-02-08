using System.ComponentModel.DataAnnotations;

namespace PrismBot.SDK.Models;

public class Group
{
    private List<string> _permissions = new List<string>();

    [Key] public string GroupName { get; set; }

    public Group? Parent { get; set; }
    public string Permissions { get; set; }
    
    public bool RefreshedPermission = false;

    public Group(string groupName, Group? parent)
    {
        GroupName = groupName;
        Parent = parent;
        Permissions = string.Empty;
    }

    private Group() {}

    private void RefreshPermission()
    {
        if (Permissions.Length == 0)
        {
            _permissions = new List<string>();
            return;
        }
        _permissions = new List<string>(Permissions.Split(","));
    }

    public void AddPermission(string permission)
    {
        if (!RefreshedPermission)
        {
            RefreshPermission();
        }
        if (_permissions.Contains(permission)) return;
        _permissions.Add(permission);
        Permissions = string.Join(",", _permissions);
    }

    public void RemovePermission(string permission)
    {
        if (!RefreshedPermission)
        {
            RefreshPermission();
        }
        if (_permissions.Contains(permission)) _permissions.Remove(permission);
        Permissions = string.Join(",", _permissions);
    }

    public bool HasPermission(string permission)
    {
        if (!RefreshedPermission)
        {
            RefreshPermission();
        }
        if (_permissions.Contains(permission)) return true;
        if (Parent != null) return Parent.HasPermission(permission);
        return false;
    }
    
    public List<string> GetPermissions()
    {
        if (!RefreshedPermission)
        {
            RefreshPermission();
        }
        return _permissions;
    }
}