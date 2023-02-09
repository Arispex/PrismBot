using System.ComponentModel.DataAnnotations;

namespace PrismBot.SDK.Models;

public class Player
{
    private List<string> _permissions = new List<string>();

    [Key] public long QQ { get; set; }

    public string UserName { get; set; }
    public long Coins { get; set; }
    public Group Group { get; set; }
    public string Permissions { get; set; }

    public bool RefreshedPermission = false;

    public Player(long qq, string userName, Group group)
    {
        QQ = qq;
        UserName = userName;
        Group = group;
        Permissions = string.Empty;
        Coins = 0;
    }
    
    private Player() {}

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
        if (permission == String.Empty) return true;
        if (_permissions.Contains(permission)) return true;

        if (Group.HasPermission(permission)) return true;
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