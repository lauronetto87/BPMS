using AutoMapper;
using Satelitti.Model;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class AbstractServiceBase<DTO, TInfo, TRepo> : FluentValidationServiceBase, IServiceBase<DTO, TInfo>
       where DTO : class
       where TInfo : BaseInfo
       where TRepo : IRepositoryBase<TInfo>
    {
        internal readonly TRepo _repository;
        internal readonly IMapper _mapper;

        public AbstractServiceBase(TRepo repository, IMapper mapper)
        {
            this._repository = repository;
            this._mapper = mapper;
        }

        public async virtual Task<ResultContent> Delete(DTO info)
        {
            await _repository.Delete(_mapper.Map<TInfo>(info));
            return Result.Success<TInfo>(null);
        }

        public async virtual Task<ResultContent> Delete(int id)
        {
            TInfo info = await _repository.Get(id);
            if (info == null)
                return Result.Error(ExceptionCodes.ENTITY_TO_DELETE_NOT_FOUND);

            await _repository.Delete(info);
            return Result.Success<TInfo>(null);
        }

        public async virtual Task<ResultContent<TInfo>> Get(int id)
        {
            return Result.Success(await _repository.Get(id));
        }

        public async virtual Task<ResultContent<int>> Insert(DTO info)
        {
            return Result.Success(await _repository.Insert(_mapper.Map<TInfo>(info)));
        }

        public async virtual Task<List<TInfo>> ListAsync(Dictionary<string, string> pFilters = null)
        {
            List<TInfo> resultList = await _repository.ListAsync();

            if (pFilters != null && pFilters.Any())
            {
                resultList = Filter(resultList, pFilters);
            }
            return resultList;
        }

        public virtual List<TInfo> Filter(List<TInfo> list, Dictionary<string, string> pFilters = null)
        {
            return list.Where(x => Filter(x, pFilters)).ToList();
        }

        private bool Filter(TInfo item, Dictionary<string, string> pFilters)
        {
            foreach (KeyValuePair<string, string> filter in pFilters)
            {
                PropertyInfo prop = typeof(TInfo).GetProperty(filter.Key);
                if (prop == null)
                    throw new Exception($"Property '{filter.Key}' is not a known value to filter '{typeof(TInfo).Name}' class");

                object propValue = prop.GetValue(item);
                if (prop.PropertyType == typeof(string))
                {
                    if (propValue.ToString() != filter.Value)
                        return false;
                }
                else
                {
                    object valueToCompare = Convert.ChangeType(filter.Value, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    if (!propValue.Equals(valueToCompare))
                        return false;
                }
            }
            return true;
        }

        public async virtual Task<ResultContent> Update(int id, DTO info)
        {
            ResultContent<TInfo> _infoResult = await this.Get(id);
            if (_infoResult.Success && _infoResult.Value == null)
                return Result.Error(ExceptionCodes.ENTITY_TO_UPDATE_NOT_FOUND);

            TInfo currMap = _mapper.Map<TInfo>(info);
            await _repository.Update(_mapper.Map(currMap, _infoResult.Value));
            return Result.Success<TInfo>(null);
        }

        public async Task Update(TInfo info)
        {
            await _repository.Update(info);
        }
    }
}
