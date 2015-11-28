using System.Collections.Generic;
using Microsoft.AspNet.Mvc.Rendering;

namespace SyncTube.ViewModels.Manage
{
    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }

        public ICollection<SelectListItem> Providers { get; set; }
    }
}