// 存放单个角色的基础信息
using UnityEngine;

[System.Serializable]
public class RoleInfo
{
    [Header("角色名称（显示在对话框旁）")]
    public string roleName;

    [Header("角色立绘/头像")]
    public Sprite roleHead;

    [Header("角色显示位置")]
    public RolePosition rolePosition;
}

// 枚举：限定角色只能显示在左侧或右侧，防止填错
public enum RolePosition
{
    Left,
    Right
}