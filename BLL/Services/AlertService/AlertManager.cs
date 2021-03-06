﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using IDAL.Interfaces;
using IDAL.Interfaces.IManagers;
using IDAL.Models;

namespace BLL.Services.AlertService
{
    public class AlertManager: IAlertManager
    {
        public AlertManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IUnitOfWork _unitOfWork { get; }

        #region Get all alerts

        public async Task<List<Alert>> GetNew()
        {
            return await _unitOfWork.AlertRepository.GetNewAlerts();
        }

        public async Task<List<Alert>> GetNew(CancellationToken cancellationToken)
        {
            return await _unitOfWork.AlertRepository.GetNewAlerts(cancellationToken);
        }
        #endregion

        #region Get advisor alerts

        public async Task<List<Alert>> GetAdvisorAlerts(Guid userId)
        {
            ICollection<Admin> adminList = await _unitOfWork.AdminRepository.GetAll();
            ICollection<Alert> newAlerts = await _unitOfWork.AlertRepository.GetNewAlerts();
            
            List<Alert> alertList = 
                (from аlert in newAlerts from admin in adminList
                 where аlert.UserId == admin.AdminId || аlert.UserId== userId
                 select аlert).ToList();
            return alertList;
        }

        public async Task<List<Alert>> GetAdvisorAlerts(CancellationToken cancellationToken, Guid userId)
        {
            return await _unitOfWork.AlertRepository.GetAdvisorAlerts(cancellationToken, userId);
        }

        #endregion

        #region Find employee

        public async Task<Employee> FindEmployeeAsync(Alert alert)
        {
            if (alert.EmployeeId != Guid.Empty)
            {
                return await _unitOfWork.EmployeeRepository.FindById(alert.EmployeeId);
            }
            return null;
        }

        public async Task<Employer> FindEmployerAsync(Alert alert)
        {
            if (alert.EmployerId != Guid.Empty)
            {
                return await _unitOfWork.EmployerRepository.FindById(alert.EmployerId);
            }
            return null;
        }

        #endregion

        #region Create

        public void Create(Alert alert)
        {
            if (alert == null)
            {
                throw new ArgumentException("alert is null. Wrong parameter");
            }

            _unitOfWork.AlertRepository.Add(alert);
            _unitOfWork.SaveChanges();
        }

        public async Task<int> CreateAsync(Alert alert)
        {
            if (alert == null)
            {
                throw new ArgumentException("alert is null. Wrong parameter");
            }
            _unitOfWork.AlertRepository.Add(alert);
            return await _unitOfWork.SaveChanges();
        }

        public async Task<int> CreateAsync(CancellationToken cancellationToken, Alert alert)
        {
            if (alert == null)
            {
                throw new ArgumentException("alert is null. Wrong parameter");
            }

            _unitOfWork.AlertRepository.Add(alert);
            return await _unitOfWork.SaveChanges(cancellationToken);

        }
        #endregion

        #region Comment

        //public void Comment(Alert alert, User user)
        //{

        //}

        //public async Task<int> CommentAsync(Alert alert, User user)
        //{

        //}
        //public async Task<int> CommentAsync(CancellationToken cancellationToken, Alert alert, User user)
        //{

        //}

        #endregion

        #region Approve

        public async Task<int> Approve(Alert alert)
        {
            if (alert.AlertType==AlertType.Employee_Add)
            {
                Employee employee = await this.FindEmployeeAsync(alert);
                if (employee != null)
                {
                    employee.IsApprove = true;
                    _unitOfWork.EmployeeRepository.Update(employee);
                }
            }
            alert.AlertIsDeleted = true;
            alert.AlertUpdateTS = DateTime.Now;
            _unitOfWork.AlertRepository.Update(alert);
            return await _unitOfWork.SaveChanges();
        }

       
        #endregion

        #region Clean all

        //public void Clean()
        //{

        //}

        //public async Task<int> CleanAsync()
        //{

        //}

        //public async Task<int> CleanAsync(CancellationToken cancellationToken)
        //{

        //}

        #endregion

        #region GetAlert
        public Alert GetAlert(Guid alertid)
        {
            if (alertid == null)
            {
                throw new ArgumentException("alert is null. Wrong parameters");
            }
            return  _unitOfWork.AlertRepository.FindAlertById(alertid).Result;
        }

        public  async Task<Alert> GetAlertAsync(Guid alertid)
        {
            if (alertid == null)
            {
                throw new ArgumentException("alert is null. Wrong parameters");
            }
            return await _unitOfWork.AlertRepository.FindById(alertid);
        }

        public async Task<Alert> GetAlertAsync(CancellationToken cancellationToken, Guid alertid)
        {
            if (alertid == null)
            {
                throw new ArgumentException("alert is null. Wrong parameters");
            }
            return await _unitOfWork.AlertRepository.FindById(cancellationToken,alertid);
        }
#endregion

    }
}
