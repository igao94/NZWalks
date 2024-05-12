using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTOs;
using NZWalks.API.Models.Entites;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IWalkRepository _walkRepository;
        private readonly IMapper _mapper;

        public WalksController(IWalkRepository walkRepository, IMapper mapper)
        {
            _walkRepository = walkRepository ?? throw new ArgumentNullException(nameof(walkRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [Authorize(Roles = "Writer, Reader")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var walks = await _walkRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true,
                pageNumber, pageSize);

            return Ok(_mapper.Map<List<WalkDto>>(walks));
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Writer, Reader")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walk = await _walkRepository.GetByIdAsync(id);

            if (walk == null) return NotFound();

            return Ok(_mapper.Map<WalkDto>(walk));
        }

        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            var walk = _mapper.Map<Walk>(addWalkRequestDto);

            await _walkRepository.CreateAsync(walk);

            var walkDto = _mapper.Map<WalkDto>(walk);

            return CreatedAtAction(nameof(GetById), new { walkDto.Id }, walkDto);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromBody] UpdateWalkRequestDto updateWalkRequestDto,
            [FromRoute] Guid id)
        {
            var walk = _mapper.Map<Walk>(updateWalkRequestDto);

            await _walkRepository.UpdateAsync(walk, id);

            if (walk == null) return NotFound();

            return Ok(_mapper.Map<WalkDto>(walk));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var walk = await _walkRepository.DeleteAsync(id);

            if (walk == null) return NotFound();

            return Ok(_mapper.Map<WalkDto>(walk));
        }
    }
}