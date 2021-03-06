﻿using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace Domain.Tests
{
    [TestFixture]
    public class ReportServiceTests
    {
        private ReportService _reportService;
        private Mock<IReportBuilder> _reportBuilder;
        private Mock<IReportSender> _reportSender;

        [SetUp]
        public void SetUp()
        {
            _reportBuilder = new Mock<IReportBuilder>();
            _reportSender = new Mock<IReportSender>();

            _reportService = new ReportService(_reportBuilder.Object, _reportSender.Object);

        }

        [Test]
        public void SendReports_WithClientId_BuildReportsForTheClient()
        {
            int clientId = 3;

            _reportBuilder.Setup(x => x.BuildReports(It.IsAny<int>())).Returns(new List<Report>
            {
                new Report()
            });

            
            var result = _reportService.SendReports(clientId);

            _reportBuilder.Verify(x => x.BuildReports(clientId), Times.Once);
        }
        
        [Test]
        public void SendReports_WhenCall_RetrunsNumberOfReports()
        {
            int clientId = 3;

            _reportBuilder.Setup(x => x.BuildReports(3)).Returns(new List<Report>
            {
                new Report(),
                new Report()
            });

            
            var result = _reportService.SendReports(clientId);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void SendReports_WhenCall_SentAllReports()
        {
            _reportBuilder.Setup(x => x.BuildReports(1)).Returns(new List<Report>
            {
                new Report(),
                new Report()
            });

            var result = _reportService.SendReports(1);

            _reportSender.Verify(x => x.Send(It.IsAny<Report>()), Times.Exactly(2));
        }
        
        [Test]
        public void SendReports_WhenCall_SentAllCreatedReports()
        {
            var report1 = new Report();
            var report2 = new Report();
            _reportBuilder.Setup(x => x.BuildReports(1)).Returns(new List<Report>
            {
                report1,
                report2
            });

            var result = _reportService.SendReports(1);

            _reportSender.Verify(x => x.Send(report1), Times.Once);
            _reportSender.Verify(x => x.Send(report2), Times.Once);
        }


        [Test]
        public void SendReports_NoReportsCreated_SendSpecialReportToManager()
        {
            _reportBuilder.Setup(x => x.BuildReports(1)).Returns(new List<Report>());
            var specialReport = new SpecialReport();
            _reportBuilder.Setup(x => x.BuildSpecialReport()).Returns(specialReport);

            var result = _reportService.SendReports(1);

            _reportSender.Verify(x => x.Send(specialReport), Times.Once);
        }

        
        [Test]
        public void SendReports_ReportsCreated_DoesNotBuildSpecialReport()
        {
            var clientId = 1;
            _reportBuilder.Setup(x => x.BuildReports(clientId)).Returns(new List<Report>{ new Report()});
          
            var result = _reportService.SendReports(clientId);

            _reportBuilder.Verify(x=>x.BuildSpecialReport(), Times.Never);
        }
    }


}