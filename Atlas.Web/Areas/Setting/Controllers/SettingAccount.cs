using Atlas.Core.Interfaces;
using Atlas.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Web.Areas.Setting.Controllers
{
    [Area("Setting")]
    [Authorize]
    public class SettingAccountController : Controller
    {
        
    }
}