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
    public class MstMachineServiceImpl : IMstMachineService
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
        public MstMachineServiceImpl(IBaseRepositoryWrapper repo
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

        public async Task<GenericResponseResult<MstMachineDto>> SearchMstMachine(MstMachineSearchImpl condition, bool blnExport = false)
        {

            var iqResult = from e in _repo.MstMachine.GetAll().AsNoTracking()
                           where (string.IsNullOrEmpty(condition.HMI_NO) || e.HmiNo.ToLower() == condition.HMI_NO.ToLower())
                           && (string.IsNullOrEmpty(condition.MACHINE_NO) || e.MachineNo.ToLower() == condition.MACHINE_NO.ToLower())
                           && (string.IsNullOrEmpty(condition.MACHINE_NAME) || e.MachineName.ToLower().Contains(condition.MACHINE_NAME.ToLower()))
                           select _mapper.Map<MstMachineDto>(e);


            //count total record
            int total1 = await iqResult.CountAsync();
            // int total2 = await iqResult2.CountAsync();
            int total = total1;// + total2;

            if (total <= 0)
            {
                return new GenericResponseResult<MstMachineDto>();
            }
            //count total page
            int totalPage1 = (int)Math.Ceiling(total1 / (double)condition.PageSize);
            int totalPage = (int)Math.Ceiling(total / (double)condition.PageSize);

            //Index start from 0
            if (totalPage - 1 < condition.PageIndex)
            {
                condition.PageIndex = totalPage - 1;
            }

            List<MstMachineDto> lstData = new List<MstMachineDto>();
            //export get all data
            lstData = await iqResult.ToListAsync();

            return new GenericResponseResult<MstMachineDto>(lstData, total, condition.PageIndex, condition.PageSize);
        }


        #endregion

    }
}
