﻿namespace Ape.Volo.ViewModel.Jwt;

/// <summary>
/// 权限
/// </summary>
public class UrlAccessControlVo
{
    /// <summary>
    /// 请求路径
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 请求方法
    /// </summary>
    public string Method { get; set; }
}
