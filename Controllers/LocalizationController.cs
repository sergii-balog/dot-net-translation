using dotNetWebApiTranslation.Services.Localization;
using Microsoft.AspNetCore.Mvc;

namespace dotNetWebApiTranslation.Controllers;

[ApiController]
[Route("[controller]")]
public class LocalizationController: ControllerBase
{
  private readonly IResourceLocalizer _localizer;

  public LocalizationController(IResourceLocalizer localizer)
  {
    _localizer = localizer;
  }


  [HttpGet]
  [Route("/get-resource")]
  public IActionResult GetResource(string key)
  {
    return Ok(_localizer[key]);
  }

  [HttpGet]
  [Route("/get-scoped-resource")]
  public IActionResult GetScopedResource(string key, string scope)
  {
    return Ok(_localizer[key, scope]);
  }

  [HttpGet]
  [Route("/get-localized-html")]
  public IActionResult GetHtmlResource(string name)
  {
    var result = $@"
      <head>
        <title>{_localizer["common.title"]}</title>
      </head>
      <body>
          <h1>{_localizer["common.hello"].Replace("{name}",name)}</h1>
          <p>{_localizer["candidate.title","candidates"]}</p>
          <p>{_localizer["candidate.hello","candidates"].Replace("{name}",name)}</p>
      </body>
";
    return Ok(result);
  }

}
