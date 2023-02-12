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

    /// <summary>
    /// 添加权限
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
    /// 移除权限
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
        if (permission == String.Empty) return true;
        if (_permissions.Contains(permission)) return true;
        if (Parent != null) return Parent.HasPermission(permission);
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