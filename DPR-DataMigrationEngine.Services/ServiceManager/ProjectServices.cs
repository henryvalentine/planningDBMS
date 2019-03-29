using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class ProjectServices
	{
        public List<Project> GetAllOrderedProjects(int itemsPerPage, int pageNumber, out int dataCount)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Projects.Any())
                    {
                        dataCount = db.Projects.Count();
                        var myObjList = db.Projects.OrderBy(m => m.Name).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).Include("Company").Include("Sector").Include("Terrain").Include("ProjectType").ToList();

                        //.OrderBy(sortColumn).Skip((page - 1) *  pageSize).Take(pageSize + 1).ToList()

                        if (!myObjList.Any())
                        {
                            return new List<Project>();
                        }
                        
                        return myObjList;
                    }
                    dataCount = 0;
                    return new List<Project>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                dataCount = 0;
                return new List<Project>();
            }
        }

       public List<Project> GetOrderedProjects()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Projects.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Project>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Project>();
            }
        }
        public long AddProjectCheckDuplicate(Project project)
        {
            try
            {
                if (project == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    //if (db.Projects.Any())
                    //{
                    //    if (db.Projects.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == project.Name.ToLower().Replace(" ", string.Empty)) > 0)
                    //    {
                    //        return -3;
                    //    }
                    //}

                   var ttr = db.Projects.Add(project);
                    db.SaveChanges();
                    return ttr.ProjectId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);

                return 0;
            }
        }
        public int UpdateProjectCheckDuplicate(Project project)
        {
            try
            {
                if (project == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    //if (db.Projects.Any())
                    //{
                    //    if (db.Projects.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == project.Name.ToLower().Replace(" ", string.Empty) && m.ProjectId != project.ProjectId) > 0)
                    //    {
                    //        return -3;
                    //    }
                    //}

                    db.Projects.Attach(project);
                    db.Entry(project).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool DeleteProjectCheckReferences(int projectId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.ProjectReps.Count(m => m.ProjectId == projectId) > 0)
                    {
                        return false;
                    }

                    if (db.ProjectMileStones.Count(m => m.ProjectId == projectId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.Projects.Where(s => s.ProjectId == projectId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.Projects.Remove(myObj[0]);
                    var txx = db.SaveChanges();
                    return txx > 0;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }
        public Project GetProject(long projectId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Projects.Where(m => m.ProjectId == projectId).Include("Company").Include("Sector").Include("Terrain").Include("ProjectType").ToList();
                    if (!myObjList.Any())
                    {
                        return new Project();
                    }
                    var project = myObjList[0];
                    project.Company = null;
                    project.Sector = null;
                    project.ProjectType = null;
                    project.Terrain = null;
                    return project;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Project();
            }
        }
        public Project GetStrippedProject(long projectId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Projects.Where(m => m.ProjectId == projectId).ToList();
                    if (!myObjList.Any())
                    {
                        return new Project();
                    }
                    var project = myObjList[0];
                    project.Company = null;
                    project.Sector = null;
                    project.ProjectType = null;
                    project.Terrain = null;
                    return project;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Project();
            }
        }
        public List<Project> GetAllOrderedProjectsByTerrain(int terrainId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Projects.Where(m => m.TerrainId == terrainId).Include("Company").Include("Sector").Include("Terrain").Include("ProjectType").ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Project>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Project>();
            }
        }
        public List<Project> GetAllOrderedProjectsByCompany(int companyId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Projects.Where(m => m.CompanyId == companyId).Include("Company").Include("Sector").Include("Terrain").Include("ProjectType").ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Project>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Project>();
            }
        }
        public List<Project> GetOrderedCompletedProjectsByMonth(DateTime date, int completionStatus)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var month = date.Month;
                    var myObjList = db.Projects.Where(m => m.DateCompleted.Value.Month == month && m.CompletionStatus == completionStatus).Include("Company").Include("Terrain").Include("ProjectType").ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Project>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Project>();
            }
        }
        public List<ProjectReportObject> GetStaticProjectReport(int itemsPerPage, int pageNumber)
        {
            try
            {
                var fxg = new List<ProjectReportObject>();
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellCompletions.Any())
                    {

                        var myObjList =
                            db.Projects.OrderBy(m => m.Name)
                                .Skip((pageNumber - 1)*itemsPerPage)
                                .Take(itemsPerPage)
                                .Include("Company")
                                .Include("Sector")
                                .Include("Terrain")
                                .Include("ProjectType")
                                .ToList();
                        if (!myObjList.Any())
                        {
                            return new List<ProjectReportObject>();
                        }

                        myObjList.ForEach(m =>
                        {
                            var description = " ";
                            if (!string.IsNullOrEmpty(m.Description))
                            {
                                description = m.Description;
                            }

                            var completionStatus = "Uncompleted";
                            if (m.CompletionStatus == 1)
                            {
                                completionStatus = "Completed";
                            }

                            var objectives = " ";
                            if (!string.IsNullOrEmpty(m.ProjectObjectives))
                            {
                                objectives = m.ProjectObjectives;
                            }
                            
                            fxg.Add(new ProjectReportObject
                            {
                                ProjectId = m.ProjectId,
                                ProjectName = m.Name,
                                TerrainName = m.Terrain.Name,
                               SectorName = m.Sector.Name,
                               Description =description,
                                ProjectTypeName = m.ProjectType.Name,
                                Duration = m.Duration.ToString(CultureInfo.InvariantCulture),
                                Cost = Convert.ToDecimal(m.Cost).ToString("#,##0"),
                               DateCompleted = m.DateCompleted.ToString(),
                               CompletionStatus = completionStatus,
                                Objectives = objectives,
                                CompanyName = m.Company.Name
                            });


                          
                            
                        });

                        return fxg;
                    }
                    return new List<ProjectReportObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<ProjectReportObject>();
            }
        }

        public List<ProjectReportObject> GetStaticProjectReportByCompany(long companyId, int itemsPerPage, int pageNumber)
        {
            try
            {
                var fxg = new List<ProjectReportObject>();
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellCompletions.Any())
                    {

                        var myObjList =
                            db.Projects.Where(x => x.CompanyId == companyId).OrderBy(m => m.Name)
                                .Skip((pageNumber - 1) * itemsPerPage)
                                .Take(itemsPerPage)
                                .Include("Company")
                                .Include("Sector")
                                .Include("Terrain")
                                .Include("ProjectType")
                                .ToList();
                        if (!myObjList.Any())
                        {
                            return new List<ProjectReportObject>();
                        }

                        myObjList.ForEach(m =>
                        {
                            var description = " ";
                            if (!string.IsNullOrEmpty(m.Description))
                            {
                                description = m.Description;
                            }

                            var completionStatus = "Uncompleted";
                            if (m.CompletionStatus == 1)
                            {
                                completionStatus = "Completed";
                            }

                            var objectives = " ";
                            if (!string.IsNullOrEmpty(m.ProjectObjectives))
                            {
                                objectives = m.ProjectObjectives;
                            }

                            fxg.Add(new ProjectReportObject
                            {
                                ProjectId = m.ProjectId,
                                ProjectName = m.Name,
                                TerrainName = m.Terrain.Name,
                                SectorName = m.Sector.Name,
                                Description = description,
                                ProjectTypeName = m.ProjectType.Name,
                                Duration = m.Duration.ToString(CultureInfo.InvariantCulture),
                                Cost = Convert.ToDecimal(m.Cost).ToString("#,##0"),
                                DateCompleted = m.DateCompleted.ToString(),
                                CompletionStatus = completionStatus,
                                Objectives = objectives,
                                CompanyName = m.Company.Name
                            });




                        });

                        return fxg;
                    }
                    return new List<ProjectReportObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<ProjectReportObject>();
            }
        }
	}


	
}
