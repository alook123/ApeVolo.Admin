using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Model;
using Ape.Volo.Core;
using Ape.Volo.Core.Utils;
using Ape.Volo.Entity.Core.Permission;
using Ape.Volo.IBusiness.Permission;
using Ape.Volo.SharedModel.Dto.Core.Permission;
using Ape.Volo.SharedModel.Queries.Common;
using Ape.Volo.SharedModel.Queries.Permission;

namespace Ape.Volo.Business.Permission;

/// <summary>
/// Api路由服务
/// </summary>
public class ApisService : BaseServices<Apis>, IApisService
{
    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="createUpdateApisDto"></param>
    /// <returns></returns>
    public async Task<OperateResult> CreateAsync(CreateUpdateApisDto createUpdateApisDto)
    {
        if (await TableWhere(a => a.Url == createUpdateApisDto.Url).AnyAsync())
        {
            return OperateResult.Error(ValidationError.IsExist(createUpdateApisDto, nameof(createUpdateApisDto.Url)));
        }

        var apis = App.Mapper.MapTo<Apis>(createUpdateApisDto);
        var result = await AddAsync(apis);
        return OperateResult.Result(result);
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="createUpdateApisDto"></param>
    /// <returns></returns>
    public async Task<OperateResult> UpdateAsync(CreateUpdateApisDto createUpdateApisDto)
    {
        var oldApis =
            await TableWhere(x => x.Id == createUpdateApisDto.Id).FirstAsync();
        if (oldApis.IsNull())
        {
            return OperateResult.Error(ValidationError.NotExist(createUpdateApisDto, LanguageKeyConstants.Api,
                nameof(createUpdateApisDto.Id)));
        }

        if (oldApis.Url != createUpdateApisDto.Url &&
            await TableWhere(a => a.Url == createUpdateApisDto.Url).AnyAsync())
        {
            return OperateResult.Error(ValidationError.IsExist(createUpdateApisDto, nameof(createUpdateApisDto.Url)));
        }

        var apis = App.Mapper.MapTo<Apis>(createUpdateApisDto);
        var result = await UpdateAsync(apis);
        return OperateResult.Result(result);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public async Task<OperateResult> DeleteAsync(HashSet<long> ids)
    {
        var apis = await TableWhere(x => ids.Contains(x.Id)).ToListAsync();
        if (apis.Count < 1)
        {
            return OperateResult.Error(ValidationError.NotExist());
        }

        var result = await LogicDelete<Apis>(x => ids.Contains(x.Id));
        return OperateResult.Result(result);
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="apisQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    public async Task<List<Apis>> QueryAsync(ApisQueryCriteria apisQueryCriteria, Pagination pagination)
    {
        var queryOptions = new QueryOptions<Apis>
        {
            Pagination = pagination,
            ConditionalModels = apisQueryCriteria.ApplyQueryConditionalModel()
        };
        return await TablePageAsync(queryOptions);
    }

    /// <summary>
    /// 查询全部
    /// </summary>
    /// <returns></returns>
    public async Task<List<Apis>> QueryAllAsync()
    {
        return await Table.ToListAsync();
    }

    /// <summary>
    /// 批量创建
    /// </summary>
    /// <param name="apis"></param>
    /// <returns></returns>
    public async Task<OperateResult> CreateAsync(List<Apis> apis)
    {
        var result = await AddAsync(apis);
        return OperateResult.Result(result);
    }
}
