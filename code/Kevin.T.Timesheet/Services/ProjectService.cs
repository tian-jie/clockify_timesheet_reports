﻿using Infrastructure.Core.Data;
using Kevin.T.Timesheet.Interfaces;
using Kevin.T.Timesheet.Entities;
using System.Collections.Generic;
using System.Linq;
using Kevin.T.Timesheet.ModelsView;

namespace Kevin.T.Timesheet.Services
{
    public class ProjectService : BaseService<Project>, IProjectService
    {
        IEstimateEffortService _estimateEffortService;
        ITaskToProjectMappingService _taskToProjectMappingService;

        public ProjectService(IEstimateEffortService estimateEffortService, ITaskToProjectMappingService taskToProjectMappingService)
            : base("Timesheet")
        {
            _estimateEffortService = estimateEffortService;
            _taskToProjectMappingService = taskToProjectMappingService;
        }


        public List<Project> GetAllProjects()
        {
            var projects = Repository.Entities.Where(a => a.IsDeleted != true).ToList();

            // 再加上其他以task为项目的项目
            var taskAsProjects = _taskToProjectMappingService.All();
            foreach (var taskAsProject in taskAsProjects)
            {
                projects.Add(new Project()
                {
                    Id = 0,
                    Name = taskAsProject.ProjectName,
                    Gid = taskAsProject.TaskGid
                });
            }

            return projects;
        }

        public Project GetProjectById(string Gid)
        {
            var project = Repository.Entities.FirstOrDefault(a => a.IsDeleted != true && a.Gid == Gid);

            if(project == null)
            {
                var taskAsProject = _taskToProjectMappingService.Repository.Entities.FirstOrDefault(a => a.IsDeleted != true && a.TaskGid == Gid);
                project = new Project()
                {
                    Id = 0,
                    Name = taskAsProject.ProjectName,
                    Gid = taskAsProject.TaskGid
                };
            }

            return project;
        }

        public ProjectAccountingView GetProjectEstimatedEffortById(string Gid)
        {
            var efforts = _estimateEffortService.Repository.Entities.Where(a => a.ProjectGid == Gid);
            var estimatedSpentManHour = efforts.Sum(a => a.Effort);
            var estimatedSpentManHourRate = efforts.Sum(a => (a.Effort * a.RoleRate));

            return new ProjectAccountingView()
            {
                ProjectGid = Gid,
                EstimatedSpentManHour = estimatedSpentManHour,
                EstimatedSpentManHourRate = estimatedSpentManHourRate
            };
        }

        public ProjectAccountingView AccountProject(string Gid)
        {
            // 第一步，获取这个项目的所有clockify信息
            //_timeEntryService

            return null;
        }
    }
}
