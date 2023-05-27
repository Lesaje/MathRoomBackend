using MathRoom.Backend.Data;
using Microsoft.AspNetCore.Mvc;

namespace MathRoom.Backend.Controllers;

[ApiController]
public class ApiController : ControllerBase
{
    public ApiController(DataContext dataContext)
    {
        DataContext = dataContext;
    }

    public readonly DataContext DataContext;
}