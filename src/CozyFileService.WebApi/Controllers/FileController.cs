using CozyFileService.Application.Features.ManageFiles.Commands.CreateFile;
using CozyFileService.Application.Features.ManageFiles.Commands.DeleteFile;
using CozyFileService.Application.Features.ManageFiles.Commands.UpdateFile;
using CozyFileService.Application.Features.ManageFiles.Queries.GetFilesList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CozyFileService.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("all", Name = "GetAllFiles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<FilesListViewModel>>> GetAllFiles()
        {
            var query = new GetFilesListQuery();
            var files = await _mediator.Send(query);

            return Ok(files);
        }

        [HttpPost("upload", Name = "UploadFile")]
        public async Task<IActionResult> Create([FromForm] IFormFile file)
        {
            if (file == null || file.Length <= 0)
                return BadRequest("Invalid file.");

            var command = new CreateFileCommand
            {
                FileName = file.FileName,
                FileType = file.ContentType,
                FileSize = file.Length,
                ContentStream = file.OpenReadStream()
            };

            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [HttpPut(Name = "UpdateFile")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult> Update([FromBody] UpdateFileCommand updateFileCommand)
        {
            await _mediator.Send(updateFileCommand);
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteFile")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult> Delete(Guid id)
        {
            var command = new DeleteFileCommand() { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
