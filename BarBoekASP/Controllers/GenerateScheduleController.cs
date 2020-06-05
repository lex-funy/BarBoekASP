﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarBoekASP.Data.MySQL;
using BarBoekASP.Data.Repositories;
using BarBoekASP.Interfaces;
using BarBoekASP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BarBoekASP.Controllers
{
    public class GenerateScheduleController : Controller
    {
        ScheduleSaveRepository scheduleSaveRepository;
        iScheduleSaveContext _ischeduldeSaveContext;
        ShiftSaveRepository shiftSaveRepository;
        iShiftSaveContext _ishiftSaveContext;

        iShiftRetrieveContext _iShiftRetrieveContext;
        ShiftRetRepository shiftRetRepository;
        iMemberRetrieveContext _iMemberRetrieveContext;
        MemberRetRepository memberRetRepository;


        public GenerateScheduleController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            _iShiftRetrieveContext = new ShiftMySQLContext(connectionString);
            shiftRetRepository = new ShiftRetRepository(_iShiftRetrieveContext);

            _iMemberRetrieveContext = new MemberMySQLContext(connectionString);
            memberRetRepository = new MemberRetRepository(_iMemberRetrieveContext);

            _ischeduldeSaveContext = new ScheduleMySQLContext(connectionString);
            scheduleSaveRepository = new ScheduleSaveRepository(_ischeduldeSaveContext);

            _ishiftSaveContext = new ShiftMySQLContext(connectionString);
            shiftSaveRepository = new ShiftSaveRepository(_ishiftSaveContext);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GenerateSchedule()
        {
            return View(new ShiftViewModel());
        }

        [HttpPost]
        public IActionResult GenerateSchedule(string month, string VoorkeurLeden)
        {
            List<MemberDTO> members = memberRetRepository.GetAll();

            ShiftViewModel shiftViewModel = new ShiftViewModel();

            List<ShiftDTO> membershifts = shiftRetRepository.GetAllShiftsForClub(month);

            foreach (ShiftDTO shift in membershifts)
            {
                scheduleSaveRepository.Shifts.Add(shift);

                scheduleSaveRepository.PlanShifts(members, VoorkeurLeden);
            }

            foreach (ShiftDTO shiftmember in scheduleSaveRepository.Shifts)
            {
                shiftSaveRepository.SaveLidShift(shiftmember);
                ShiftDetailViewModel model = new ShiftDetailViewModel();

                model.EndMoment = shiftmember.EndMoment;
                model.StartMoment = shiftmember.StartMoment;
                model.Members = shiftmember.Members;
                model.ID = shiftmember.ID;

                shiftViewModel.Shifts.Add(model);
            }

            return View(shiftViewModel);
        }

        [HttpDelete]
        public IActionResult GenerateSchedule(int Delete)
        {

            return View(new ShiftViewModel());
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ShiftDTO shift = this.shiftRetRepository.GetByID(id);

            ShiftDetailViewModel model = new ShiftDetailViewModel()
            {
                ID = shift.ID,
                Name = shift.Name,
                StartMoment = shift.StartMoment,
                EndMoment = shift.EndMoment,
                EventType = shift.EventType,
                MaxMemberCount = shift.MaxMemberCount
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ShiftViewModel shiftViewModel)
        {
            // Create dto from model
            // Save model to database

            return RedirectToAction("GenerateSchedule");
        }
    }
}