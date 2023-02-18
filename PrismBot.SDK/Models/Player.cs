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
    
    public string RegistrationTime { get; set; }
    
    public bool IsFreeze { get; set; }
    
    public bool IsSignedIn { get; set; }

    public bool RefreshedPermission = false;

    public Player(long qq, string userName, Group group)
    {
        QQ = qq;
        UserName = userName;
        Group = group;
        Permissions = string.Empty;
        Coins = 0;
        RegistrationTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        IsFreeze = false;
        IsSignedIn = false;
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
    /// <summary>
    /// 增加权限
    /// </summary>
    /// <param name="permission">权限名</param>
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

    /// <summary>
    /// 删除权限
    /// </summary>
    /// <param name="permission">权限名</param>
    public void RemovePermission(string permission)
    {
        if (!RefreshedPermission)
        {
            RefreshPermission();
        }
        if (_permissions.Contains(permission)) _permissions.Remove(permission);
        Permissions = string.Join(",", _permissions);
    }

    /// <summary>
    /// 判断是否拥有指定权限
    /// </summary>
    /// <param name="permission">需要判断的权限名</param>
    /// <returns></returns>
    public bool HasPermission(string permission)
    {
        if (!RefreshedPermission)
        {
            RefreshPermission();
        }
        if (permission == string.Empty) return true;
        if (_permissions.Contains(permission)) return true;

        if (Group.HasPermission(permission)) return true;
        return false;
    }

    /// <summary>
    /// 获取权限列表
    /// </summary>
    /// <returns>权限列表</returns>
    public List<string> GetPermissions()
    {
        if (!RefreshedPermission)
        {
            RefreshPermission();
        }
        return _permissions;
    }
}