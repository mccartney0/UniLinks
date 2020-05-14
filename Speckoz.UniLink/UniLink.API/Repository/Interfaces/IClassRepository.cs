﻿using System;
using System.Threading.Tasks;

using UniLink.API.Models;

namespace UniLink.API.Repository.Interfaces
{
    public interface IClassRepository
    {
        Task<ClassModel> AddTasAsync(ClassModel @class);

        Task<ClassModel> FindByClassIdTaskAsync(Guid classId);

        Task<ClassModel> FindByURITaskAsync(string uri);

        Task RemoveAsync(ClassModel @class);
    }
}