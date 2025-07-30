using AdnTestingSystem.Repositories.Data;
using AdnTestingSystem.Repositories.Models;
using AdnTestingSystem.Repositories.UnitOfWork;
using AdnTestingSystem.Services.Helpers;
using AdnTestingSystem.Services.Interfaces;
using AdnTestingSystem.Services.Requests;
using AdnTestingSystem.Services.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdnTestingSystem.Services.Services
{
    public class TestResultService : ITestResultService
    {
        private readonly IUnitOfWork _uow;
        private readonly AdnTestingDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailSender _email;
        public TestResultService(IUnitOfWork uow, AdnTestingDbContext context, IHttpContextAccessor httpContextAccessor, IEmailSender email)
        {
            _uow = uow;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _email = email;
        }

        public async Task SaveTestResultAsync(SaveTestResultRequest request, int currentUserId)
        {
            var now = DateTime.UtcNow;

            var testResult = new TestResult
            {
                BookingId = request.BookingId,
                Conclusion = request.Conclusion,
                CPI = request.CPI,
                Probability = request.Probability,
                CreatedById = currentUserId,
                CreatedAt = now,
                UpdatedAt = now,
                UpdatedBy = currentUserId
            };

            foreach (var item in request.LocusResults)
            {
                var detail = new TestResultDetail
                {
                    Locus = item.Locus,
                    Allele1_Person1 = item.Allele1_Person1,
                    Allele2_Person1 = item.Allele2_Person1,
                    Allele1_Person2 = item.Allele1_Person2,
                    Allele2_Person2 = item.Allele2_Person2,
                    PI = item.PI,
                    Note = item.Note,
                    CreatedAt = now,
                    CreatedBy = currentUserId,
                    UpdatedAt = now,
                    UpdatedBy = currentUserId
                };

                testResult.LocusResults.Add(detail);
            }

            _uow.TestResults.AddAsync(testResult);
            await _uow.CompleteAsync();
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == request.BookingId);
            if (booking != null)
            {
                booking.IsTestResultCreated = true;
                booking.UpdatedAt = DateTime.UtcNow;
                booking.UpdatedBy = currentUserId;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<CommonResponse<PagedResult<TestResultListResponse>>> GetTestResultsAsync(TestResultListRequest request)
        {
            var query = _uow.TestResults.Query()
                .Include(tr => tr.LocusResults)
                .Where(tr => tr.DeletedAt == null);

            if (request.TestResultId.HasValue)
            {
                query = query.Where(tr => tr.Id == request.TestResultId.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(tr => (int)tr.Status == request.Status.Value);
            }

            query = query.OrderByDescending(tr => tr.UpdatedAt);

            var paged = await PaginationHelper.ToPagedResultAsync(query, request.Page, request.PageSize);

            var items = paged.Items.Select(tr => new TestResultListResponse
            {
                Id = tr.Id,
                BookingId = tr.BookingId,
                Conclusion = tr.Conclusion,
                CPI = tr.CPI,
                Probability = tr.Probability,
                Status = (int)tr.Status,
                CreatedAt = tr.CreatedAt,
                Details = tr.LocusResults.Select(d => new TestResultDetailResponse
                {
                    Id = d.Id,
                    Locus = d.Locus,
                    Allele1_Person1 = d.Allele1_Person1,
                    Allele2_Person1 = d.Allele2_Person1,
                    Allele1_Person2 = d.Allele1_Person2,
                    Allele2_Person2 = d.Allele2_Person2,
                    PI = d.PI,
                    Note = d.Note
                }).ToList()
            }).ToList();

            return CommonResponse<PagedResult<TestResultListResponse>>.Ok(new PagedResult<TestResultListResponse>
            {
                Items = items,
                TotalItems = paged.TotalItems,
                Page = paged.Page,
                PageSize = paged.PageSize
            });
        }
    }
}
