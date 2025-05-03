﻿using Ape.Volo.Common.Attributes;
using Ape.Volo.Common.Enums;
using Ape.Volo.Entity.Base;

namespace Ape.Volo.ViewModel.Core.Permission.Menu;

/// <summary>
/// 菜单Vo
/// </summary>
[AutoMapping(typeof(Entity.Core.Permission.Menu), typeof(MenuVo))]
public class MenuVo : BaseEntityDto<long>
{
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 路径
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 权限标识
    /// </summary>
    public string Permission { get; set; }

    /// <summary>
    /// 是否IFrame
    /// </summary>
    public bool IFrame { get; set; }

    /// <summary>
    /// 组件
    /// </summary>
    public string Component { get; set; }

    /// <summary>
    /// 组件名称
    /// </summary>
    public string ComponentName { get; set; }

    /// <summary>
    /// 父级ID
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// Icon
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public MenuType Type { get; set; }

    /// <summary>
    /// 缓存
    /// </summary>
    public bool Cache { get; set; }

    /// <summary>
    /// 隐藏
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// 子节点个数
    /// </summary>
    public int SubCount { get; set; }

    /// <summary>
    /// 子节点
    /// </summary>
    public List<MenuVo> Children { get; set; }

    /// <summary>
    /// 页
    /// </summary>
    public bool Leaf => SubCount == 0;

    /// <summary>
    /// 是否有子节点
    /// </summary>
    public bool HasChildren => SubCount > 0;

    /// <summary>
    /// 标签
    /// </summary>
    public string Label
    {
        get { return Title; }
    }
}
