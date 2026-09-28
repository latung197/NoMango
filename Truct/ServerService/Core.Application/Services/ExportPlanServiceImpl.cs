using Core.Application.Constants;
using Core.Application.CustomModels;
using Core.Application.CustomModels.Dtos;
using Core.Application.CustomModels.Others;
using Core.Application.CustomModels.SearchConditions;
using Core.Application.Enum;
using Core.Application.Interface;
using Core.Domain.Entity;
using Core.Domain.Interface;
using Core.Application.Security;
using Core.Utils;
using Core.Utils.LogUtils;
using Core.Application.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core.Application.Services
{
    public partial class ExportPlanServiceImpl : IExportPlanService
    {
        //Repo
        private readonly IBaseRepositoryWrapper _repo;
        //Get config from appsettings.json if need
        private readonly IConfiguration _configuration;
        //Mapping model to entity
        private readonly CoreMapper _mapper;
        //Log
        private readonly ILoggerManager _logger;
        private readonly IUserPrincipalService _userPrincipalService;
        public ExportPlanServiceImpl(IBaseRepositoryWrapper repo
            , IConfiguration configuration
            , CoreMapper mapper
            , ILoggerManager logger
            , IUserPrincipalService userPrincipalService)
        {
            _repo = repo;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
            _userPrincipalService = userPrincipalService;
        }

    }
}
