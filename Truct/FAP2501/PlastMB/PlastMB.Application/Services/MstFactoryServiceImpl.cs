using PlastMB.Application.CustomModels;
using PlastMB.Application.CustomModels.Dtos;
using PlastMB.Application.CustomModels.SearchConditions;
using PlastMB.Application.Enum;
using PlastMB.Application.Interface;
using PlastMB.Domain.Entity;
using PlastMB.Domain.Interface;
using PlastMB.Utils;
using PlastMB.Utils.LogUtils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PlastMB.Application.Services
{
    public class MstFactoryServiceImpl : IMstFactoryService
    {
        #region Properties
        //Repo
        private readonly IBaseRepositoryWrapper _repo;
        //Get config from appsettings.json if need
        private readonly IConfiguration _configuration;
        //Mapping model to entity
        private readonly IMapper _mapper;
        //Log
        private readonly ILoggerManager _logger;
        #endregion
        #region Constructor
        public MstFactoryServiceImpl(IBaseRepositoryWrapper repo
            , IConfiguration configuration
            , IMapper mapper
            , ILoggerManager logger)
        {
            _repo = repo;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion
        #region Search

        public async Task<GenericResponseResult<MstFactoryDto>> SearchMstFactory(MstFactorySearchImpl condition, bool blnExport = false)
        {

            var iqResult = from e in _repo.MstFactory.GetAll().AsNoTracking()
                           where (string.IsNullOrEmpty(condition.FACTORY_NAME) || e.FactoryName.ToLower().Contains(condition.FACTORY_NAME.ToLower()))
                           && (string.IsNullOrEmpty(condition.FACTORY_CD) || e.FactoryCd.ToString().ToLower().Contains(condition.FACTORY_CD.ToLower()))
                           select _mapper.Map<MstFactoryDto>(e);

            //count total record
            int total1 = await iqResult.CountAsync();
            int total = total1;// + total2;

            if (total <= 0)
            {
                return new GenericResponseResult<MstFactoryDto>();
            }
            //count total page
            int totalPage1 = (int)Math.Ceiling(total1 / (double)condition.PageSize);
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);

            //Index start from 0
            if (totalPage - 1 < condition.PageIndex)
            {
                condition.PageIndex = totalPage - 1;
            }

            List<MstFactoryDto> lstData = new List<MstFactoryDto>();
            List<MstFactoryDto> lstData2 = new List<MstFactoryDto>();
            //export get all data
            lstData = await iqResult.ToListAsync();
            return new GenericResponseResult<MstFactoryDto>(lstData, total, condition.PageIndex, condition.PageSize);
        }


        #endregion

    }
}
