using System;
using System.Collections.Generic;

namespace BouvetWebApp.Interfaces
{
    public interface IOrganizationRepository
    {
        public List<string> GetOrgTypes();
    }
}