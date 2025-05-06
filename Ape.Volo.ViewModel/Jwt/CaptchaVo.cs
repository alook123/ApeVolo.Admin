﻿namespace Ape.Volo.ViewModel.Jwt;

/// <summary>
/// 验证码
/// </summary>
public class CaptchaVo
{
    /// <summary>
    /// 图片base64
    /// </summary>
    public string Img { get; set; }

    /// <summary>
    /// 验证码ID
    /// </summary>
    public string CaptchaId { get; set; }

    /// <summary>
    /// 是否显示验证码
    /// </summary>
    public bool ShowCaptcha { get; set; }
}
