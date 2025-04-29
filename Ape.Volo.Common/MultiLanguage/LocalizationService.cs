﻿using System.Collections.Generic;
using Ape.Volo.Common.MultiLanguage.Contract;
using Ape.Volo.Common.MultiLanguage.Resources;
using Microsoft.Extensions.Localization;

namespace Ape.Volo.Common.MultiLanguage;

public class LocalizationService : ILocalizationService
{
    private readonly IStringLocalizer _localizer;

    public LocalizationService(IStringLocalizer<Language> localizer)
    {
        _localizer = localizer;
    }


    public string R(string name)
    {
        return _localizer[name];
    }

    public string R(string name, params object[] arguments)
    {
        return _localizer[name, arguments];
    }

    public IEnumerable<LocalizedString> RAll(bool includeParentCultures)
    {
        return _localizer.GetAllStrings();
    }
}
